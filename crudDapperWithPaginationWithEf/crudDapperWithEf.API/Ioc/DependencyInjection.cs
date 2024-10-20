using crudDapperWithEf.API.Connection;
using crudDapperWithEf.API.DTOs.Validations.Client;
using crudDapperWithEf.API.DTOs.Validations.Product;
using crudDapperWithEf.API.Repositories;
using crudDapperWithEf.API.Repositories.Interfaces;
using crudDapperWithEf.API.Services;
using crudDapperWithEf.API.Services.Exports;
using crudDapperWithEf.API.Services.Interfaces;
using FluentValidation;

namespace crudDapperWithEf.API.Ioc
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<ConnectionFactory>();
            services.AddScoped<IGenericRepository, GenericRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();

            return services;
        }

        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IExcelExportService, ExcelExportService>();

            return services;
        }

        public static IServiceCollection RegisterFluentValidations(this IServiceCollection services)
        {
           services.AddValidatorsFromAssemblyContaining<ClientDTOValidation>();
           services.AddValidatorsFromAssemblyContaining<ProductDTOValidation>();
           services.AddValidatorsFromAssemblyContaining<ClientUpdateDTOValidation>();
           services.AddValidatorsFromAssemblyContaining<ProductUpdateDTOValidation>();

           return services;
        }
    }
}