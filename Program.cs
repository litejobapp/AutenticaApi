using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DocumentValidator;
using AutenticaAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IValidator<NovoLead>, NovoLeadValidator>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddDbContext<SaasClientDb>(options =>

options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); ;
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.MapPost("/", NovoLead);
app.MapGet("/", GetAllClientes);

app.Run();


static async Task<IResult> GetAllClientes([FromServices] SaasClientDb db)
{
    return TypedResults.Ok(await db.SaasClient.ToArrayAsync());
}

static async Task<IResult> NovoLead(IValidator<NovoLead> validator, [FromBody] NovoLead lead, [FromServices] SaasClientDb db, [FromServices] IEmailSender mailSender)
{
    //db.SaasClient.Add(cliente);
    //await db.SaveChangesAsync();
    var validationResult = await validator.ValidateAsync(lead);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }
    if(db.SaasClient.FirstOrDefault(x=>x.Email.ToLower() == lead.Email.ToLower()) != null)
    {
        return Results.BadRequest("Email já cadastrado!");
    }
    db.SaasClient.Add(new SaasClient()
    {
        Cnpj = lead.Cnpj,
        ChaveValidacao = Guid.NewGuid().ToString(),
        DataCriacao = DateTime.Now,
        DataValidacao = null,
        Email = lead.Email,
        EmailValidado = false,
        Fantasia = lead.Nome,
        Identificador = Guid.NewGuid().ToString(),
        Nome = lead.Nome,
        TipoUsuario = "A"//administrador,

    });
    await db.SaveChangesAsync();

    await mailSender.EnviaEmail(new Email() { Body = "teste", Subject = "teste", To = "julianomiquelleto@gmail.com" });

    return TypedResults.Created($"/cliente/{lead.Email}", lead);
}


public class NovoLeadValidator : AbstractValidator<NovoLead> {

    public NovoLeadValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email é obrigatório")
                     .EmailAddress().WithMessage("Email inválido");
        RuleFor(x => x.Nome).NotEmpty().WithMessage("Nome é obrigatório");
        RuleFor(x => x.Fone).NotEmpty().WithMessage("Fone é obrigatório");
        RuleFor(x => x.Cnpj).NotEmpty().WithMessage("Cnpj é obrigatório");
        RuleFor(x => CnpjValidation.Validate(x.Cnpj)).Equal(true)
                .WithMessage("O CNPJ fornecido é inválido.");
    }
}



public record NovoLead {
    
    public required string Email{ get; set; }
    public required string Nome { get; set; }
    public required string Cnpj { get; set; }
    public required string Fone { get; set; }
    public string Obs { get; set; }

}
public record SaasClient

{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public required string Fantasia { get; set; }
    public string? Cnpj { get; set; }
    public required string Identificador { get; set; }
    public required string Email { get; set; }
    public required bool EmailValidado { get; set; }
    public required DateTime DataCriacao { get; set; }
    public  DateTime? DataValidacao { get; set; }
    public required string ChaveValidacao { get; set; }
    public required string TipoUsuario { get; set; }
}

 class SaasClientDb : DbContext
{
    public SaasClientDb(DbContextOptions<SaasClientDb> options)
    : base(options) { }

    public DbSet<SaasClient> SaasClient => Set<SaasClient>();
}


