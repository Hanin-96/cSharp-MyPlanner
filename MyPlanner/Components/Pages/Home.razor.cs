using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MyPlanner.Data;

namespace MyPlanner.Components.Pages
{
    public partial class Home : ComponentBase
    {
        //Todo lista med alla objekt
        public List<Todo>? ReadTodoList { get; set; }

        //Ny todo post
        public Todo? NewTodo { get; set; }

        //Visa formulär på sidan
        public bool ShowCreate { get; set; }

        //Edit get set
        public bool EditRecord { get; set; }

        //Skapa ny Todo vid uppdatering
        public Todo? TodoToUpdate { get; set; }

        // EditingId
        public int EditingId { get; set; }

        //Från klassen TodoDataContext, referens till databasanslutning
        private TodoDataContext? dataContext;

        protected override async Task OnInitializedAsync()
        {
            ShowCreate = false;
            await ShowTodoList();
        }

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

        //Metod - Skapa ny todo
        public async Task CreateNewTodo()
        {
            dataContext ??= await TodoDataContextFactory.CreateDbContextAsync();

            if (NewTodo is not null)
            {
                dataContext?.Todos.Add(NewTodo);
                await dataContext?.SaveChangesAsync();
                ReadTodoList = await dataContext.Todos.OrderBy(todo => todo.Priority).ToListAsync();
            }
            ShowCreate = false;
        }

        //extrahera och visa todo lista
        public async Task ShowTodoList()
        {
            dataContext ??= await TodoDataContextFactory.CreateDbContextAsync();

            if (dataContext is not null)
            {
                // Sortera efter prioritet och skriv ut todo posts
                ReadTodoList = await dataContext.Todos.OrderBy(todo => todo.Priority).ToListAsync();
            }

        }

        //Metod - Sätta id på existerande todo
        public async Task ShowEditTodoForm(Todo readTodoList)
        {
            dataContext ??= await TodoDataContextFactory.CreateDbContextAsync();
            TodoToUpdate = dataContext.Todos.FirstOrDefault(x => x.Id == readTodoList.Id);
            EditingId = readTodoList.Id;
            EditRecord = true;
        }

        //Stänga edit mode
        public async Task CloseEditForm()
        {
            EditRecord = false;
        }

        //Metod - Uppdatera todo enligt id
        public async Task TodoUpdate()
        {
            EditRecord = false;
            dataContext ??= await TodoDataContextFactory.CreateDbContextAsync();

            if (dataContext is not null && TodoToUpdate is not null)
            {
                dataContext.Todos.Update(TodoToUpdate);
                await dataContext.SaveChangesAsync();

            }
            ReadTodoList = await dataContext.Todos.OrderBy(todo => todo.Priority).ThenBy(todo => todo.Title).ToListAsync();
            //await dataContext.DisposeAsync();
        }

        //Metod - Radera todo post
        public async Task DeleteTodoPost(Todo readTodoList)
        {
            dataContext ??= await TodoDataContextFactory.CreateDbContextAsync();
            if (dataContext is not null && readTodoList is not null)
            {
                dataContext.Todos.Remove(readTodoList);
                await dataContext.SaveChangesAsync();
            }
            await ShowTodoList();
        }


    }
}

