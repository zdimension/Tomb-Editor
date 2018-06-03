﻿using FastColoredTextBoxNS;
using System.Collections.Generic;

namespace ScriptEditor
{
	public class AutocompleteItems
	{
		public static List<AutocompleteItem> GetItems()
		{
			// Get key words
			List<string> objects = SyntaxKeyWords.Headers();
			List<string> keyValues = SyntaxKeyWords.KeyValues();
			List<string> unknown = SyntaxKeyWords.Unknown();

			// Create the Autocomplete list
			List<AutocompleteItem> items = new List<AutocompleteItem>();

			// Add key words to the Autocomplete list
			foreach (var item in objects)
			{
				items.Add(new AutocompleteItem(item));
			}

			foreach (var item in keyValues)
			{
				items.Add(new AutocompleteItem(item));
			}

			foreach (var item in unknown)
			{
				items.Add(new AutocompleteItem(item));
			}

			// Add these additional key words
			items.Add(new AutocompleteItem("ENABLED"));
			items.Add(new AutocompleteItem("DISABLED"));

			return items;
		}
	}
}
