﻿namespace Alura.CoisasAFazer.Services.Handlers
{
	public class CommandResult
	{
		public bool IsSuccess { get;  }

		public CommandResult(bool success)
		{
			this.IsSuccess = success;
		}
	}
}