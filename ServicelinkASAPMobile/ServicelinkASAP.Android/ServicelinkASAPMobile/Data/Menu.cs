using System;

namespace ServicelinkASAP
{
	public class Menu
	{
		public string Type { get; set; }

		// List of produce items for that type, such as bananas, apricots, plums, apples
		public MenuItem[] MenuItems { get; set; }
	}

	// Represents a produce item ("bananas", "carrots", etc.) within a type of produce:
	public class MenuItem
	{
		// Name of produce item ("Bananas", "Carrots", etc.)
		public string Name { get; set; }

		// How many units of this produce item are in stock:
		public int Count { get; set; }

	}
}

