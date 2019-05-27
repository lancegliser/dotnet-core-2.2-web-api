using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Service.Models;
using System.Linq;

namespace Service.Models
{
    class TodoRepository
    {
        private TodoContext _context;
    
        public TodoRepository(TodoContext context)
        {
            this._context = context;
        }
    
        public async Task<List<TodoItem>> GetAll()
        {
            return await _context.TodoItems.ToListAsync();
        }
    
        // public Person Add(Person person)
        // {
        //     personDataContext.People.Add(person);
        //     personDataContext.SaveChanges();
        //     return person;
        // }
    }

}
