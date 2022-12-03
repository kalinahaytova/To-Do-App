using System.Collections.Generic;

namespace To_Do_App.Models.ViewModels;

	public class TodoViewModel
	{
		public List<TodoItem> TodoList { get; set; }
		public TodoItem Todo { get; set; }

	}


