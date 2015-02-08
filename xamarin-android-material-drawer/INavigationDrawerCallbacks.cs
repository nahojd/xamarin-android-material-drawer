
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;

namespace MaterialDrawer
{
	public interface INavigationDrawerCallbacks
	{
		void OnNavigationDrawerItemSelected(int position);
	}
	
}
