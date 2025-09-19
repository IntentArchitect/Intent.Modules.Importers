using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Common.Interfaces;
using JsonImportTests.Domain.Repositories;
using JsonImportTests.Domain.Repositories.ECommerce.Catalog.Categories;
using JsonImportTests.Domain.Repositories.ECommerce.Catalog.Products;
using JsonImportTests.Domain.Repositories.ECommerce.ECommerceInventory;
using JsonImportTests.Domain.Repositories.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Education.University.Students;
using JsonImportTests.Domain.Repositories.Finance.TradingPlatform.Assets;
using JsonImportTests.Domain.Repositories.Finance.TradingPlatform.Portfolios;
using JsonImportTests.Domain.Repositories.Finance.TradingPlatform.Transactions;
using JsonImportTests.Domain.Repositories.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Healthcare.Patients;
using JsonImportTests.Domain.Repositories.Healthcare.Staff;
using JsonImportTests.Infrastructure.Configuration;
using JsonImportTests.Infrastructure.Persistence;
using JsonImportTests.Infrastructure.Repositories;
using JsonImportTests.Infrastructure.Repositories.ECommerce.Catalog.Categories;
using JsonImportTests.Infrastructure.Repositories.ECommerce.Catalog.Products;
using JsonImportTests.Infrastructure.Repositories.ECommerce.ECommerceInventory;
using JsonImportTests.Infrastructure.Repositories.EdgeCases.ComplexTypes;
using JsonImportTests.Infrastructure.Repositories.Education.University.Academic;
using JsonImportTests.Infrastructure.Repositories.Education.University.EducationEnrollment;
using JsonImportTests.Infrastructure.Repositories.Education.University.Students;
using JsonImportTests.Infrastructure.Repositories.Finance.TradingPlatform.Assets;
using JsonImportTests.Infrastructure.Repositories.Finance.TradingPlatform.Portfolios;
using JsonImportTests.Infrastructure.Repositories.Finance.TradingPlatform.Transactions;
using JsonImportTests.Infrastructure.Repositories.Healthcare.Clinical;
using JsonImportTests.Infrastructure.Repositories.Healthcare.Patients;
using JsonImportTests.Infrastructure.Repositories.Healthcare.Staff;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace JsonImportTests.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCosmosRepository();
            services.AddScoped<ICustomerRepository, CustomerCosmosDBRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceCosmosDBRepository>();
            services.AddScoped<ICategoryCategoryRepository, categoryCategoryCosmosDBRepository>();
            services.AddScoped<IProductRepository, ProductCosmosDBRepository>();
            services.AddScoped<IInventoryItemRepository, InventoryItemCosmosDBRepository>();
            services.AddScoped<IComplexEntityRepository, ComplexEntityCosmosDBRepository>();
            services.AddScoped<ICourseCourseRepository, courseCourseCosmosDBRepository>();
            services.AddScoped<IEnrollmentEnrollmentRepository, enrollmentEnrollmentCosmosDBRepository>();
            services.AddScoped<IStudentStudentRepository, studentStudentCosmosDBRepository>();
            services.AddScoped<IAssetRepository, AssetCosmosDBRepository>();
            services.AddScoped<IPortfolioRepository, PortfolioCosmosDBRepository>();
            services.AddScoped<ITransactionRepository, TransactionCosmosDBRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentCosmosDBRepository>();
            services.AddScoped<IMedicalRecordRepository, MedicalRecordCosmosDBRepository>();
            services.AddScoped<IPatientRepository, PatientCosmosDBRepository>();
            services.AddScoped<IPractitionerRepository, PractitionerCosmosDBRepository>();
            services.AddScoped<CosmosDBUnitOfWork>();
            services.AddScoped<ICosmosDBUnitOfWork>(provider => provider.GetRequiredService<CosmosDBUnitOfWork>());
            services.AddMassTransitConfiguration(configuration);
            return services;
        }
    }
}