﻿using System;

namespace Drastic.UI
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class QueryPropertyAttribute : Attribute
	{
		public string Name { get; }
		public string QueryId { get; }

		public QueryPropertyAttribute(string name, string queryId)
		{
			Name = name;
			QueryId = queryId;
		}
	}
}
