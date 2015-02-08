
using System;
using System.Collections.Generic;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace MaterialDrawer
{

	public class NavigationDrawerAdapter : RecyclerView.Adapter
	{
		private List<NavigationItem> data;
		private int selectedPosition;
		private int touchedPosition = -1;

		public INavigationDrawerCallbacks NavigationDrawerCallbacks { get; set; }

		public NavigationDrawerAdapter (List<NavigationItem> data)
		{
			this.data = data;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder (ViewGroup parent, int viewType)
		{
			var view = LayoutInflater.From (parent.Context).Inflate (Resource.Layout.DrawerRow, parent, false);

			var viewHolder = new ViewHolder (view);
			return viewHolder;
		}

		public override void OnBindViewHolder (RecyclerView.ViewHolder holder, int position)
		{
			var viewHolder = (ViewHolder)holder;

			viewHolder.TextView.Text = data [position].Text;
			viewHolder.TextView.SetCompoundDrawablesWithIntrinsicBounds (data [position].Drawable, null, null, null);

			// Ugly way to make sure that the touch and click event handlers are only added once
			// In the original java code, setOnClickListener is used instead of an event, which replaces the click handler every time,
			// but this is not an option in .NET, since we can't inline the implementation of the IOnClickListener, and thus cannot
			// access the position in the listener. 
			// Really, the touch and click event handlers should be added in a method that is not called on every change.
			if (!viewHolder.ItemView.HasOnClickListeners) {
				viewHolder.ItemView.Touch += (sender, e) => {
					switch (e.Event.Action) {
					case MotionEventActions.Down:
						TouchPosition (position);
						e.Handled = false;
						break;
					case MotionEventActions.Cancel:
						TouchPosition (-1);
						e.Handled = false;
						break;
					case MotionEventActions.Move:
						e.Handled = false;
						break;
					case MotionEventActions.Up:
						TouchPosition (-1);
						e.Handled = false;
						break;
					default:
						e.Handled = true;
						break;
					}
				};

				//TODO: selected menu position, change layout accordingly
				viewHolder.ItemView.Click += (sender, e) => {
					if (NavigationDrawerCallbacks != null)
						NavigationDrawerCallbacks.OnNavigationDrawerItemSelected (position);
				};
			}

			if (selectedPosition == position || touchedPosition == position) {
				viewHolder.ItemView.SetBackgroundColor (viewHolder.ItemView.Context.Resources.GetColor (Resource.Color.selected_gray));
			} else {
				viewHolder.ItemView.SetBackgroundColor (Android.Graphics.Color.Transparent);
			}
		}

		private void TouchPosition (int position)
		{
			var lastPosition = touchedPosition;
			touchedPosition = position;
			if (lastPosition >= 0)
				NotifyItemChanged (lastPosition);
			if (position >= 0)
				NotifyItemChanged (position);
		}

		public void SelectPosition (int position)
		{
			var lastPosition = selectedPosition;
			selectedPosition = position;
			NotifyItemChanged (lastPosition);
			NotifyItemChanged (position);
		}

		public override int ItemCount { get { return data != null ? data.Count : 0; } }

		public class ViewHolder : RecyclerView.ViewHolder
		{
			public TextView TextView { get; set; }

			public ViewHolder (View itemView) : base (itemView)
			{
				TextView = itemView.FindViewById<TextView> (Resource.Id.item_name);
			}
		}
	}
}
