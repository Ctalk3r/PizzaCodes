package md5244a88b3ef4508729787e3308fb782f7;


public class XFGlossSwitchRenderer
	extends md51558244f76c53b6aeda52c8a337f2c37.SwitchRenderer
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("XFGloss.Droid.Renderers.XFGlossSwitchRenderer, XFGloss", XFGlossSwitchRenderer.class, __md_methods);
	}


	public XFGlossSwitchRenderer (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == XFGlossSwitchRenderer.class)
			mono.android.TypeManager.Activate ("XFGloss.Droid.Renderers.XFGlossSwitchRenderer, XFGloss", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public XFGlossSwitchRenderer (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == XFGlossSwitchRenderer.class)
			mono.android.TypeManager.Activate ("XFGloss.Droid.Renderers.XFGlossSwitchRenderer, XFGloss", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public XFGlossSwitchRenderer (android.content.Context p0)
	{
		super (p0);
		if (getClass () == XFGlossSwitchRenderer.class)
			mono.android.TypeManager.Activate ("XFGloss.Droid.Renderers.XFGlossSwitchRenderer, XFGloss", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
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
