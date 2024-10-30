using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MyPlanner.Data;

namespace MyPlanner.Components.Pages
{
    public partial class Home : ComponentBase
    {
        private List<Todo>? readTodoList;
        private Todo? newTodo;
        private bool showCreate;
        private bool editRecord;
        private Todo? todoToUpdate;
        private Todo? backUpTodo;
        private int editingId;
        private bool hasError;

        //Todo lista med alla objekt
        public List<Todo>? ReadTodoList { get { return readTodoList; } set { readTodoList = value; } }

        //Ny todo post
        public Todo? NewTodo { get { return newTodo; } set { newTodo = value; } }

        //Visa formulär på sidan
        public bool ShowCreate { get { return showCreate; } set { showCreate = value; } }

        //Edit get set
        public bool EditRecord { get { return editRecord; } set { editRecord = value; } }

        //Skapa ny Todo vid uppdatering
        public Todo? TodoToUpdate { get { return todoToUpdate; } set { todoToUpdate = value; } }

        //Skapa ny backup Todo för att kunna ångra ändringar
        public Todo? BackUpTodo { get { return backUpTodo; } set { backUpTodo = value; } }

        // EditingId
        public int EditingId { get { return editingId; } set { editingId = value; } }

        //error-msg
        public bool HasError { get { return hasError; } set { hasError = value; } }

        //Från klassen TodoDataContext, referens till databasanslutning
        private TodoDataContext? dataContext;

        protected override async Task OnInitializedAsync()
        {
            ShowCreate = false;
            HasError = false;
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

            if (IsFormValid(NewTodo))
            {
                dataContext?.Todos.Add(NewTodo);
                await dataContext?.SaveChangesAsync();
                ReadTodoList = await dataContext.Todos.OrderBy(todo => todo.Priority).ToListAsync();
                ShowCreate = false;
                //Reset error
                HasError = false;
            }
            else
            {
                HasError = true;
            }

        }

        private bool IsFormValid(Todo todoToValidate)
        {
            return todoToValidate is not null && todoToValidate.Priority != 0 && 
                todoToValidate.Description is not null && todoToValidate.Description != "" &&
                todoToValidate.Title is not null && todoToValidate.Title != "" && todoToValidate.Category != "";
        }

        //extrahera och visa todo lista
        public async Task ShowTodoList()
        {
            dataContext ??= await TodoDataContextFactory.CreateDbContextAsync();

            if (dataContext is not null)
            {
                // Sortera efter prioritet och skriv ut todo posts
                ReadTodoList = await dataContext.Todos
                   .OrderBy(todo => todo.Status)
                   .ThenBy(todo => todo.Priority)
                   .ThenBy(todo => todo.Title)
                   .ToListAsync();
            }

        }

        //Metod - Sätta id på existerande todo
        public async Task ShowEditTodoForm(Todo readTodoList)
        {
            dataContext ??= await TodoDataContextFactory.CreateDbContextAsync();
            TodoToUpdate = dataContext.Todos.FirstOrDefault(x => x.Id == readTodoList.Id);
            if(todoToUpdate is not null)
            {
                backUpTodo = new Todo
                {
                    Id = todoToUpdate.Id,
                    Title = todoToUpdate.Title,
                    Description = todoToUpdate.Description,
                    Priority = todoToUpdate.Priority,
                    Category = todoToUpdate.Category,
                    Status = todoToUpdate.Status
                };
            }
            EditingId = readTodoList.Id;
            EditRecord = true;
        }

        //Stänga edit mode
        public async Task CloseEditForm()
        {
            if( backUpTodo is not null && todoToUpdate is not null)
            {
                todoToUpdate.Title = backUpTodo.Title;
                todoToUpdate.Description = backUpTodo.Description;
                todoToUpdate.Priority = backUpTodo.Priority;
                todoToUpdate.Category = backUpTodo.Category;
                todoToUpdate.Status = backUpTodo.Status;
            }
            EditRecord = false;
        }

        //Metod - Uppdatera todo enligt id
        public async Task TodoUpdate()
        {

            HasError = false;
            dataContext ??= await TodoDataContextFactory.CreateDbContextAsync();

            if (dataContext is not null && IsFormValid(TodoToUpdate))
            {
                dataContext.Todos.Update(TodoToUpdate);
                await dataContext.SaveChangesAsync();
                ReadTodoList = await dataContext.Todos
                    .OrderBy(todo => todo.Priority)
                    .ThenBy(todo => todo.Title)
                    .OrderBy(todo => todo.Status)
                    .ToListAsync();
                //await dataContext.DisposeAsync();
                EditRecord = false;
            }
            else
            {
                HasError = true;
            }

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

        public async Task MarkTodoStatus(Todo todo)
        {
            dataContext ??= await TodoDataContextFactory.CreateDbContextAsync();
            //Toggla todo post status
                todo.Status = !todo.Status;
                dataContext.Todos.Update(todo);
                await dataContext.SaveChangesAsync();
                await ShowTodoList();

        
        }


    }
}

