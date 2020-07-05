using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public ValuesController(DataContext dataContext)
        {
            _dataContext = dataContext;

        }
        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
           // return new string[] { "value1", "value2" };
           var values= await _dataContext.Values.ToListAsync();
           return Ok(values);

        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetValue(int id)
        {
           var value= await _dataContext.Values.FirstOrDefaultAsync(x=>x.Id==id);
           return Ok(value);
        }

        [HttpPost]
        // POST api/values
        public void Post([FromBody] string value)
        {
        }
        [HttpPut]
        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete]
        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}