using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V4.Widget;

namespace MaterialDrawer
{
	[Activity (Label = "MaterialDrawer", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AppTheme")]
	public class MainActivity : ActionBarActivity, INavigationDrawerCallbacks
	{
		private Toolbar toolbar;
		private NavigationDrawerFragment navigationDrawerFragment;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.ActivityMain);

			toolbar = FindViewById<Toolbar> (Resource.Id.toolbar_actionbar);
			SetSupportActionBar (toolbar);

			SupportActionBar.SetDisplayHomeAsUpEnabled (true);

			navigationDrawerFragment = FragmentManager.FindFragmentById<NavigationDrawerFragment> (Resource.Id.fragment_drawer);
			navigationDrawerFragment.Setup (Resource.Id.fragment_drawer, FindViewById<DrawerLayout> (Resource.Id.drawer), toolbar);
		}

		public void OnNavigationDrawerItemSelected (int position)
		{
			Toast.MakeText (this, "Menu item selected -> " + position, ToastLength.Short).Show ();
		}

		public override void OnBackPressed ()
		{
			if (navigationDrawerFragment.IsDrawerOpen)
				navigationDrawerFragment.CloseDrawer ();
			else
				base.OnBackPressed ();
		}
	}
}


