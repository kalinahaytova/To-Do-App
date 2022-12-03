using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using To_Do_App.Models;
using To_Do_App.Models.ViewModels;

namespace To_Do_App.Controllers;

public class ToDoController : Controller
{
    private readonly ILogger<ToDoController> _logger;

    public ToDoController(ILogger<ToDoController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var todoListViewModel = GetAllTodos();
        return View(todoListViewModel);
    }

    [HttpGet]
    public JsonResult PopulateForm(int id)
    {
        var todo = GetById(id);
        return Json(todo);
    }

    internal TodoItem GetById(int id)
    {
        TodoItem todo = new();

        using (var connection =
               new SqlConnection("Data Source=(localdb)\\mssqllocaldb"))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText = $"SELECT * FROM todo Where Id = '{id}'";

                using (var reader = tableCmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        todo.Id = reader.GetInt32(0);
                        todo.Name = reader.GetString(1);
                    }
                    else
                    {
                        return todo;
                    }
                };
            }
        }

        return todo;
    }

    internal TodoViewModel GetAllTodos()
    {
        List<TodoItem> todoList = new();

        using (var connection =
               new SqlConnection("Data Source=(localdb)\\mssqllocaldb"))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText = "SELECT * FROM todo";

                using (var reader = tableCmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            todoList.Add(
                                new TodoItem
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1)
                                });
                        }
                    }
                    else
                    {
                        return new TodoViewModel
                        {
                            TodoList = todoList
                        };
                    }
                };
            }
        }

        return new TodoViewModel
        {
            TodoList = todoList
        };
    }

    public RedirectResult Insert(TodoItem todo)
    {
        using (SqlConnection con =
               new SqlConnection("Data Source=(localdb)\\mssqllocaldb"))
        {
            using (var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = $"INSERT INTO todo (name) VALUES ('{todo.Name}')";
                try
                {
                    tableCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        return Redirect("http://localhost:5041");
    }

    [HttpPost]
    public JsonResult Delete(int id)
    {
        using (var connection =
               new SqlConnection("Data Source=(localdb)\\mssqllocaldb"))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText = $"DELETE from todo WHERE Id = '{id}'";
                tableCmd.ExecuteNonQuery();
            }
        }

        return Json(new { });

    }

    public RedirectResult Update(TodoItem todo)
    {
        using (SqlConnection con =
               new SqlConnection("Data Source=(localdb)\\mssqllocaldb"))
        {
            using (var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = $"UPDATE todo SET name = '{todo.Name}' WHERE Id = '{todo.Id}'";
                try
                {
                    tableCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        return Redirect("http://localhost:5041");
    }
}
