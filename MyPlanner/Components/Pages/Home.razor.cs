using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MyPlanner.Data;

namespace MyPlanner.Components.Pages
{
    public partial class Home : ComponentBase
    {
        //Todo lista med alla objekt
        public List<Todo>? ReadTodoList { get; set; }

        public Todo? NewTodo { get; set; }

        //Visa formulär på sidan
        public bool ShowCreate { get; set; }

        //Edit get set
        public bool EditRecord { get; set; }

        //Skapa ny Todo vid uppdatering
        public Todo? TodoToUpdate { get; set; }

        // EditingId
        public int EditingId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ShowCreate = false;
            await ShowTodoList();
        }

        private TodoDataContext? _context;


        //Visa formulär
        public void ShowCreateForm()
        {
            NewTodo = new Todo();
            ShowCreate = true;
        }

        //Metod- Ångra skapa en ny todo
        public void CancelForm()
        {
            ShowCreate = false;
        }

        //Skapa ny todo
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

        //extrahera och visa todo lista
        public async Task ShowTodoList()
        {
            _context ??= await TodoDataContextFactory.CreateDbContextAsync();

            if (_context is not null)
            {
                ReadTodoList = await _context.Todos.ToListAsync();
            }

        }

        //Metod - Sätta id på existerande todo
        public async Task ShowEditTodoForm(Todo readTodoList)
        {
            _context ??= await TodoDataContextFactory.CreateDbContextAsync();
            TodoToUpdate = _context.Todos.FirstOrDefault(x => x.Id == readTodoList.Id);
            EditingId = readTodoList.Id;
            EditRecord = true;
        }

        public async Task CloseEditForm()
        {
            EditRecord = false;
        }

        //Metod - Uppdatera todo enligt id
        public async Task TodoUpdate()
        {
            EditRecord = false;
            _context ??= await TodoDataContextFactory.CreateDbContextAsync();

            if (_context is not null)
            {
                if (TodoToUpdate is not null) _context.Todos.Update(TodoToUpdate);
                await _context.SaveChangesAsync();
                await _context.DisposeAsync();
            }
        }
    }
}

