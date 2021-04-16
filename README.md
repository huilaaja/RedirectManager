<h1>Redirect Manager</h1>

<p>Redirect Manager is simple, portable, extendable, open source redirect tool for Episerver projects.</p>

<h2>Description</h2>
<p>It's built to be as minimalistic as possible and as an example of simple Episerver add-on. 
It requires only 7 files for business logic and data access, 2 controllers and 1 view.</p>

<h2>New 2.0 version has a bit smaller appearance and multi-site support</h2>
<p>Preview:</p>
<p><img src="https://raw.githubusercontent.com/huilaaja/RedirectManager/master/images/redirect-manager-6.png" /></p>

<h2>Features</h2>
<ul>
	<li>Easily create redirects to any URLs or to Episerver pages.</li>
	<li>Wild card rules.</li>
	<li>Reordering and prioritizing rules.</li>
	<li>Multi-site support.</li>
	<li>Allow moving and changing URLs of Episerver pages and the redirects still works.</li>
	<li>All redirects are HTTP 301 (Moved permanently), because search engines only follow this kind of redirects.</li>
	<li>Access restrictions allow usage of rule manager to only administrators.</li>
	<li>And the most important: It's open Source and it's yours to extend and manipulate! <a href="https://github.com/solita" target="_blank">Solita &lt;3 Open Source!</a></li>
</ul>

<h2>The key features actually are what this add-on is NOT</h2>
<ul>
	<li><b>No CSS or styling</b>; <br/>There isn't even a single line of CSS or styling in project. 
		Frameworks as Bootstrap give enough styles for simple solutions and it's responsive OOTB.</li>
	<li><b>No NuGet packaging</b>; <br/>It's not a NuGet package because NuGet packages aren't agile enough. It's easier to copy&paste changes.</li>
	<li><b>No DDS</b>; <br/>We have come to conclusion that Dynamic Data Storage isn't scalable and functional for our purposes.</li>
	<li><b>No Dojo Toolkit, No Dijit</b>; <br/>Dojo framework is way too large and complicated framework for simple solutions like this.</li>
	<li><b>No extra controllers and models</b>; <br/>MVC is nice concept, but controllers and models are often not important.
		Coding some trivial logic to views will your project more agile.</li>
	<li><b>No REST</b>; <br/>Normally we would have used AngularJS and Web API, but with administration tools it's not necessary.</li>
	<li><b>No Translations</b>; <br/>Administers normally do not need localizations, so why waste of time and energy.</li>
	<li><b>No Unit Tests</b>; <br/>There is no point of testing trivial things and unit testing != no bugs.</li>
</ul>

<h2>Redirection rules</h2>
<p>Here's couple of examples what kind of rules are possible.</p>
<p><img src="https://raw.githubusercontent.com/huilaaja/RedirectManager/master/images/redirect-manager-2.png" /></p>


<h2>Minimum Requirements</h2>
<ul>
	<li>Episerver 7 MVC project with C#7</li>
	<li>Entity Framework</li>
</ul>

<h2>Installation instructions</h2>
<ol>
	<li>Install Entity Framework from NuGet.<br/>
   https://www.nuget.org/packages/EntityFramework</li>
	<li>Copy files into your project</li>
	<li>Add .MapMvcAttributeRoutes() to RegisterRoutes override in Global.asax.cs</li>
	<li>Apply manually Web.Config transformations</li>
	<li>Go to www.yourproject.com/Admin/RedirectManager</li>
</ol>
<p><img src="https://raw.githubusercontent.com/huilaaja/RedirectManager/master/images/redirect-manager-1.png" /></p>


<h2>Instructions for usage</h2>
<p><img src="https://raw.githubusercontent.com/huilaaja/RedirectManager/master/images/redirect-manager-3.png" /></p>

<h2>File structuce</h2>
<p><img src="https://raw.githubusercontent.com/huilaaja/RedirectManager/master/images/redirect-manager-4.png" /></p>

<h2>Basic 404 redirect logic</h2>
<p><img src="https://raw.githubusercontent.com/huilaaja/RedirectManager/master/images/redirect-manager-5.png" /></p>

<h2>Auto wire up</h2>
<p>You can automatically populate the RedirectManager with data on following events from editors, moving pages in structure, renaming url segment and deleting from Waste Basket. Just put this in your InitializableModule: <br/>
  <pre>var events = ServiceLocator.Current.GetInstance&lt;ContentEvents&gt;();
            events.MovingContent += RedirectKeeper.Page_Moving;
            events.PublishingContent += RedirectKeeper.UrlSegment_Changed;
            events.DeletedContent += RedirectKeeper.Page_Deleted;</pre>
</p>