using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServicelinkASAP.Android
{
	public class ExpendableListViewAdapter:BaseExpandableListAdapter
	{
		private readonly Context _context;
		private readonly List<Menu> _menu;
	
		public ExpendableListViewAdapter (Context context, List<Menu> menu)
		{
			_context = context;
			_menu = menu;
		}

		public override bool HasStableIds{
			get { return true; }
		}

		public override long GetGroupId(int groupPosition)
		{
			// The index of the group is used as its ID:
			return groupPosition;
		}

		public override int GroupCount
		{
		
			get { return _menu.Count; }
		}

		public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
		{
			// Recycle a previous view if provided:
			var view = convertView;

			// If no recycled view, inflate a new view as a simple expandable list item 1:
			if (view == null)
			{
				var inflater = _context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
				view = inflater.Inflate(Android.Resource.Layout.ExpandableListItem1, null);
			}

			// Grab the produce object ("vegetables", "fruits", etc.) at the group position:
			var menu = _menu[groupPosition];

			// Get the built-in first text view and insert the group name ("Vegetables", "Fruits", etc.):
			TextView textView = view.FindViewById<TextView>(Android.Resource.Id.MenuTitle);
			textView.Text = menu.Type;

			return view;
		}

		public override Java.Lang.Object GetGroup(int groupPosition)
		{
			return null;
		}

		public override long GetChildId(int groupPosition, int childPosition)
		{
				// The index of the child is used as its ID:
			return childPosition;
		}

		public override int GetChildrenCount(int groupPosition)
		{
			// Return the number of children (produce item objects) in the group (produce object):
			var menu = _menu[groupPosition];
			return menu.MenuItems.Length;
		}

		public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
		{
			// Recycle a previous view if provided:
			var view = convertView;

			// If no recycled view, inflate a new view as a simple expandable list item 2:
			if (view == null)
			{
				var inflater = _context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
				view = inflater.Inflate(Android.Resource.Layout.ExpandableListItem2, null);
			}

			// Grab the produce object ("vegetables", "fruits", etc.) at the group position:
			var menu = _menu[groupPosition];

			// Extract the produce item object ("bananas", "apricots", etc.) at the child position:
			var menuItem = menu.MenuItems[childPosition];

			// Get the built-in first text view and insert the child name ("Bananas", "Apricots", etc.):
			TextView textView = view.FindViewById<TextView>(Android.Resource.Id.txtListItemDetail);
			textView.Text = menuItem.Name;

			// Reuse the textView to insert the number of produce units into the child's second text field:
		
			textView.Text += " ( " + menuItem.Count.ToString() + " )";

			return view;
		}

		public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
		{
			return null;
		}

		public override bool IsChildSelectable(int groupPosition, int childPosition)
		{
			return true;
		}
			
	
	}
}

