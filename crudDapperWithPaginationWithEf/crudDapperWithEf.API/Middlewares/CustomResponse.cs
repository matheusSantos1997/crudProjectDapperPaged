using crudDapperWithEf.API.Shared;
using Microsoft.AspNetCore.Mvc;

namespace crudDapperWithEf.API.Middlewares
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomResponse : ProducesResponseTypeAttribute
    {
        public CustomResponse(int statusCode) : base(typeof(CustomResult), statusCode){}
    }
}