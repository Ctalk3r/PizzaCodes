package md5dbbea7083a8902e4e8093965ad2e31cc;


public class XFGlossPaintDrawable
	extends android.graphics.drawable.PaintDrawable
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("XFGloss.Droid.Drawables.XFGlossPaintDrawable, XFGloss", XFGlossPaintDrawable.class, __md_methods);
	}


	public XFGlossPaintDrawable ()
	{
		super ();
		if (getClass () == XFGlossPaintDrawable.class)
			mono.android.TypeManager.Activate ("XFGloss.Droid.Drawables.XFGlossPaintDrawable, XFGloss", "", this, new java.lang.Object[] {  });
	}


	public XFGlossPaintDrawable (int p0)
	{
		super (p0);
		if (getClass () == XFGlossPaintDrawable.class)
			mono.android.TypeManager.Activate ("XFGloss.Droid.Drawables.XFGlossPaintDrawable, XFGloss", "Android.Graphics.Color, Mono.Android", this, new java.lang.Object[] { p0 });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
