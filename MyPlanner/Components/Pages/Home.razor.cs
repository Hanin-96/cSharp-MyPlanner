using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MyPlanner.Data;

namespace MyPlanner.Components.Pages
{
    public partial class Home : ComponentBase
    {
        public List<Todo>? ReadTodoList { get; set; }
        //Visa formulär på sidan
        public bool ShowCreate { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ShowCreate = false;
        }

        private TodoDataContext? _context;

        public Todo? NewTodo { get; set; }

        //Visa formulär
        public void ShowCreateForm()
        {
            NewTodo = new Todo();
            ShowCreate = true;
        }

        public async Task CreateNewTodo()
        {
            _context ??= await TodoDataContextFactory.CreateDbContextAsync();

            if (NewTodo is not null)
            {
                _context?.Todos.Add(NewTodo);
                await _context?.SaveChangesAsync();
            }
            ShowCreate = false;
        }

        //extrahera todo lista
        public async Task ShowTodoList()
        {
            _context ??= await TodoDataContextFactory.CreateDbContextAsync();

            if (_context is not null)
            {
                ReadTodoList = await _context.Todos.ToListAsync();
            }

            if (_context is not null) await _context.DisposeAsync();
        }
    }
}

