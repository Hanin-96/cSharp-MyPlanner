using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MyPlanner.Data;

namespace MyPlanner.Components.Pages
{
    //Home är återanvändbar komponent i blazor, därav arvet från ComponentBase
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

        //Körs när sidan startas, en del av blazor livscykel komponent
        protected override async Task OnInitializedAsync()
        {
            ShowCreate = false;
            HasError = false;
            //Hämtar lista på alla todo poster
            await ShowTodoList();
        }

        //Visa formulär
        public void ShowCreateForm()
        {
            //Skapar ny Todo objekt
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
            //Skapar koppling till databasen
            dataContext ??= await TodoDataContextFactory.CreateDbContextAsync();

            //Om formuläret är validerad ska ny todo post lagras i databasen
            if (IsFormValid(NewTodo))
            {
                dataContext?.Todos.Add(NewTodo);
                await dataContext?.SaveChangesAsync();
                //Sorterar ordning
                ReadTodoList = await dataContext.Todos
                   .OrderBy(todo => todo.Status)
                   .ThenBy(todo => todo.Priority)
                   .ThenBy(todo => todo.Title)
                   .ToListAsync();
                ShowCreate = false;
                //Reset error
                HasError = false;
            }
            else
            {
                HasError = true;
            }

        }
        //Metod- Formulär validering vid skapande och uppdatering av todo
        private bool IsFormValid(Todo todoToValidate)
        {
            return todoToValidate is not null && todoToValidate.Priority != 0 &&
                todoToValidate.Description is not null && todoToValidate.Description != "" &&
                todoToValidate.Title is not null && todoToValidate.Title != "" && todoToValidate.Category != "";
        }

        //extrahera och visa todo lista
        public async Task ShowTodoList()
        {
            //Skapar koppling till databasen
            dataContext ??= await TodoDataContextFactory.CreateDbContextAsync();

            //Om koppling finns
            if (dataContext is not null)
            {
                // Sortera efter status, prioritet och titel och skriv ut todo posts
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
            //Skapar koppling till databasen
            dataContext ??= await TodoDataContextFactory.CreateDbContextAsync();
            //Hämtar todo baserad på Id
            TodoToUpdate = dataContext.Todos.FirstOrDefault(x => x.Id == readTodoList.Id);
            if (todoToUpdate is not null)
            {
                //Om Id hittas sparas en backup ifall man ångrar ändring
                backUpTodo = new Todo
                {
                    Id = todoToUpdate.Id,
                    Title = todoToUpdate.Title,
                    Description = todoToUpdate.Description,
                    Priority = todoToUpdate.Priority,
                    Category = todoToUpdate.Category,
                    DueDate = todoToUpdate.DueDate,
                    Status = todoToUpdate.Status
                };
            }
            //Sparar ner Id på den som ska uppdateras
            EditingId = readTodoList.Id;
            EditRecord = true;
        }

        //Stänga edit mode
        public async Task CloseEditForm()
        {
            //Ifall man ångrar ändringar, ska backupen skriva över ändringar
            if (backUpTodo is not null && todoToUpdate is not null)
            {
                todoToUpdate.Title = backUpTodo.Title;
                todoToUpdate.Description = backUpTodo.Description;
                todoToUpdate.Priority = backUpTodo.Priority;
                todoToUpdate.Category = backUpTodo.Category;
                todoToUpdate.DueDate = backUpTodo.DueDate;
                todoToUpdate.Status = backUpTodo.Status;
            }
            EditRecord = false;
            HasError = false;
        }

        //Metod - Uppdatera todo enligt id
        public async Task TodoUpdate()
        {
            HasError = false;
            //Skapar koppling till databasen
            dataContext ??= await TodoDataContextFactory.CreateDbContextAsync();

            if (dataContext is not null && IsFormValid(TodoToUpdate))
            {
                //Uppdatera ändrat todo
                dataContext.Todos.Update(TodoToUpdate);
                await dataContext.SaveChangesAsync();
                //Hämtar och sortera listan
                ReadTodoList = await dataContext.Todos
                   .OrderBy(todo => todo.Status)
                   .ThenBy(todo => todo.Priority)
                   .ThenBy(todo => todo.Title)
                   .ToListAsync();
                EditRecord = false;
            }
            else
            {
                HasError = true;
            }

        }

        //Metod - Radera todo post
        public async Task DeleteTodoPost(Todo todoPost)
        {
            //Skapar koppling till databasen
            dataContext ??= await TodoDataContextFactory.CreateDbContextAsync();
            if (dataContext is not null && todoPost is not null)
            {
                //Radera specifik todo post
                dataContext.Todos.Remove(todoPost);
                await dataContext.SaveChangesAsync();
            }
            await ShowTodoList();
        }

        public async Task MarkTodoStatus(Todo todo)
        {
            //Skapar koppling till databasen
            dataContext ??= await TodoDataContextFactory.CreateDbContextAsync();
            //Toggla todo post status
            todo.Status = !todo.Status;
            //Uppdaterar ändring
            dataContext.Todos.Update(todo);
            await dataContext.SaveChangesAsync();
            //Hämtar sorterad lista
            await ShowTodoList();
        }


    }
}

