﻿using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Formatters;


namespace Ralfred
{
	public class BypassFormDataInputFormatter : IInputFormatter
	{
		public bool CanRead(InputFormatterContext context)
		{
			return context.HttpContext.Request.HasFormContentType;
		}

		public Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
		{
			return InputFormatterResult.SuccessAsync(null);
		}
	}
}