
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

		public NavigationDrawerAdapter(List<NavigationItem> data)
		{
			this.data = data;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.DrawerRow, parent, false);
			return new ViewHolder(view);
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var viewHolder = (ViewHolder)holder;

			viewHolder.TextView.Text = data[position].Text;
			viewHolder.TextView.SetCompoundDrawablesWithIntrinsicBounds(data[position].Drawable, null, null, null);
			viewHolder.ItemView.Touch += (sender, e) => {
				switch (e.Event.Action) {
					case MotionEventActions.Down:
						TouchPosition(position);
						break;
					case MotionEventActions.Cancel:
						TouchPosition(-1);
						break;
					case MotionEventActions.Move:
						break;
					case MotionEventActions.Up:
						TouchPosition(-1);
						break;
				}
			};

			//TODO: selected menu position, change layout accordingly
			viewHolder.ItemView.Click += (sender, e) => {
				if (NavigationDrawerCallbacks != null)
					NavigationDrawerCallbacks.OnNavigationDrawerItemSelected(position);
			};

			if (selectedPosition == position || touchedPosition == position) {
				viewHolder.ItemView.SetBackgroundColor(viewHolder.ItemView.Context.Resources.GetColor(Resource.Color.selected_gray));
			}
			else {
				viewHolder.ItemView.SetBackgroundColor(Android.Graphics.Color.Transparent);
			}
		}

		private void TouchPosition(int position)
		{
			var lastPosition = touchedPosition;
			touchedPosition = position;
			if (lastPosition >= 0)
				NotifyItemChanged(lastPosition);
			if (position >= 0)
				NotifyItemChanged(position);
		}

		public void SelectPosition(int position)
		{
			var lastPosition = selectedPosition;
			selectedPosition = position;
			NotifyItemChanged(lastPosition);
			NotifyItemChanged(position);
		}

		public override int ItemCount { get { return data != null ? data.Count : 0; } }

		public class ViewHolder : RecyclerView.ViewHolder
		{
			public TextView TextView { get; set; }

			public ViewHolder(View itemView) : base(itemView)
			{
				TextView = itemView.FindViewById<TextView>(Resource.Id.item_name);
			}
		}
	}
}
