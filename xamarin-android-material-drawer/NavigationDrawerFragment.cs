
using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace MaterialDrawer
{

	public class NavigationDrawerFragment : Fragment, INavigationDrawerCallbacks
	{
		private const string PreferencesFile = "my_app_settings";
		private const string StateSelectedPosition = "selected_navigation_drawer_position";
		private const string PrefUserLearnedDrawer = "navigation_drawer_learned";

		private INavigationDrawerCallbacks callbacks;
		private RecyclerView drawerList;
		private View fragmentContainerView;
		private bool userLearnedDrawer;
		private bool fromSavedInstanceState;
		private int currentSelectedPosition;

		public DrawerLayout DrawerLayout { get; set; }

		public ActionBarDrawerToggle ActionBarDrawerToggle { get; set; }

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			userLearnedDrawer = Convert.ToBoolean (ReadSharedSetting (Activity, PrefUserLearnedDrawer, "false"));
			if (savedInstanceState != null) {
				currentSelectedPosition = savedInstanceState.GetInt (StateSelectedPosition);
				fromSavedInstanceState = true;
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.FragmentNavigationDrawer, container, false);
			drawerList = view.FindViewById<RecyclerView> (Resource.Id.drawerList);
			var layoutManager = new LinearLayoutManager (Activity);
			layoutManager.Orientation = LinearLayoutManager.Vertical;
			drawerList.SetLayoutManager (layoutManager);
			drawerList.HasFixedSize = true;

			var navigationItems = GetMenu ();
			var adapter = new NavigationDrawerAdapter (navigationItems);
			adapter.NavigationDrawerCallbacks = this;
			drawerList.SetAdapter (adapter);
			SelectItem (currentSelectedPosition);
			return view;
		}

		public override void OnAttach (Activity activity)
		{
			base.OnAttach (activity);

			if (!(activity is INavigationDrawerCallbacks))
				throw new InvalidCastException ("Activity must implement INavigationDrawerCallbacks.");

			callbacks = activity as INavigationDrawerCallbacks;
		}



		public void Setup (int fragmentId, DrawerLayout drawerLayout, Toolbar toolbar)
		{
			fragmentContainerView = Activity.FindViewById (fragmentId);
			DrawerLayout = drawerLayout;
			drawerLayout.SetStatusBarBackgroundColor (Resources.GetColor (Resource.Color.myPrimaryDarkColor));

			ActionBarDrawerToggle = new MyActionBarDrawerToggle (Activity, drawerLayout, toolbar, Resource.String.drawer_open, Resource.String.drawer_close) {
				OnDrawerClosedCallback = (view) => {
					if (IsAdded)
						return;
					Activity.InvalidateOptionsMenu ();
				},
				OnDrawerOpenedCallback = (view) => {
					if (!IsAdded)
						return;
					if (!userLearnedDrawer) {
						userLearnedDrawer = true;
						SaveSharedSetting (Activity, PrefUserLearnedDrawer, "true");
					}

					Activity.InvalidateOptionsMenu ();
				}
			};

			if (!userLearnedDrawer && !fromSavedInstanceState)
				drawerLayout.OpenDrawer (fragmentContainerView);

			drawerLayout.Post (ActionBarDrawerToggle.SyncState);

			drawerLayout.SetDrawerListener (ActionBarDrawerToggle);
		}

		public void OpenDrawer ()
		{
			DrawerLayout.OpenDrawer (fragmentContainerView);
		}

		public void CloseDrawer ()
		{
			DrawerLayout.CloseDrawer (fragmentContainerView);
		}

		public override void OnDetach ()
		{
			base.OnDetach ();
			callbacks = null;
		}

		public List<NavigationItem> GetMenu ()
		{
			var items = new List<NavigationItem> {
				new NavigationItem { Text = "item 1", Drawable = Resources.GetDrawable (Resource.Drawable.ic_menu_check) },
				new NavigationItem { Text = "item 2", Drawable = Resources.GetDrawable (Resource.Drawable.ic_menu_check) },
				new NavigationItem { Text = "item 3", Drawable = Resources.GetDrawable (Resource.Drawable.ic_menu_check) },
			};
			return items;
		}

		void SelectItem (int position)
		{
			currentSelectedPosition = position;
			if (DrawerLayout != null) {
				DrawerLayout.CloseDrawer (fragmentContainerView);
			}
			if (callbacks != null) {
				callbacks.OnNavigationDrawerItemSelected (position);
			}
			((NavigationDrawerAdapter)drawerList.GetAdapter ()).SelectPosition (position);
		}

		public bool IsDrawerOpen {
			get { return DrawerLayout != null && DrawerLayout.IsDrawerOpen (fragmentContainerView); }
		}

		public override void OnConfigurationChanged (Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
			ActionBarDrawerToggle.OnConfigurationChanged (newConfig);
		}

		public override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
			outState.PutInt (StateSelectedPosition, currentSelectedPosition);
		}

		public void OnNavigationDrawerItemSelected (int position)
		{
			SelectItem (position);
		}

		public static string ReadSharedSetting (Context ctx, string settingName, string defaultValue)
		{
			var sharedPref = ctx.GetSharedPreferences (PreferencesFile, FileCreationMode.Private);
			return sharedPref.GetString (settingName, defaultValue);
		}

		public static void SaveSharedSetting (Context ctx, string settingName, string settingValue)
		{
			var sharedPref = ctx.GetSharedPreferences (PreferencesFile, FileCreationMode.Private);
			var editor = sharedPref.Edit ();
			editor.PutString (settingName, settingValue);
			editor.Apply ();
		}
	}

	public class MyActionBarDrawerToggle : ActionBarDrawerToggle
	{
		public MyActionBarDrawerToggle (Activity activity, DrawerLayout drawerLayout, Toolbar toolbar, int drawer_open, int drawer_close)
			: base (activity, drawerLayout, toolbar, drawer_open, drawer_close)
		{
		}

		public Action<View> OnDrawerClosedCallback { get; set; }

		public Action<View> OnDrawerOpenedCallback { get; set; }

		public override void OnDrawerClosed (View drawerView)
		{
			base.OnDrawerClosed (drawerView);

			if (OnDrawerClosedCallback != null)
				OnDrawerClosedCallback (drawerView);
		}

		public override void OnDrawerOpened (View drawerView)
		{
			base.OnDrawerOpened (drawerView);

			if (OnDrawerOpenedCallback != null)
				OnDrawerOpenedCallback (drawerView);
		}
	}
}

