<h1>Redirect Manager</h1>

<p>Redirect Manager is simple, portable, extendable, open source redirect tool for Episerver projects.</p>

<h2>Description</h2>
<p>It's built to be as minimalistic as possible and as an example of simple Episerver add-on. 
It requires only 4 files for business logic and data access, 1 controller and 1 view.</p>

<h2>Features</h2>
<ul>
	<li>Easily create redirects to any URLs or to Episerver pages.</li>
	<li>Wild card rules.</li>
	<li>Reordering and prioritizing rules.</li>
	<li>Allow moving and changing URLs of Episerver pages and the redirects still works.</li>
	<li>All redirects are HTTP 301 (Moved permanently), because search engines only follow this kind of redirects.</li>
	<li>Access restrictions allow usage of rule manager to only administrators.</li>
	<li>And the most important: It's open Source and it's yours to extend and manipulate!</li>
</ul>

<h2>The key features actually are what this add-on is NOT</h2>
<ul>
	<li>No CSS or styling; <br/>There isn't even a single line of CSS or styling in project. 
		Frameworks as Bootstrap give enough styles for simple solutions and it's responsive OOTB.</li>
	<li>No NuGet packaging; <br/>It's not a NuGet package because NuGet packages aren't agile enough. It's easier to copy&paste changes.</li>
	<li>No DDS; <br/>We have come to conclusion that Dynamic Data Storage isn't scalable and functional for our purposes.</li>
	<li>No Dojo Toolkit, No Dijit; <br/>Dojo framework is way too large and complicated framework for simple solutions like this.</li>
	<li>No REST; <br/>Normally we would have used AngularJS and Web API, but with administration tools it's not necessary.</li>
	<li>No Translations; <br/>Administers normally do not need translations, so why waste of time and energy.</li>
</ul>

<h2>Minimum Requirements</h2>
<ul>
	<li>Episerver 7 MVC project</li>
	<li>Entity Framework</li>
</ul>

<h2>Installation instructions</h2>
<ol>
	<li>Install Entity Framework from NuGet.<br/>
   https://www.nuget.org/packages/EntityFramework</li>
	<li>Copy files into your project</li>
	<li>Apply manually Web.Config transformations.</li>
	<li>Go to www.yourproject.com/Views/Admin/RedirectManager.cshtml</li>
</ol>

<p><img src="https://raw.githubusercontent.com/huilaaja/RedirectManager/master/images/redirect-manager-1.png" /></p>

<h2>Redirection rules</h2>
<p>Currently the Redirect Manager is only visible/accessible by administrators.</p>
<p>Here's couple of examples what kind of redirections are possible.</p>
<p><img src="https://raw.githubusercontent.com/huilaaja/RedirectManager/master/images/redirect-manager-2.png" /></p>

<h2>Instructions for usage</h2>
<p><img src="https://raw.githubusercontent.com/huilaaja/RedirectManager/master/images/redirect-manager-3.png" /></p>

<h2>File structuce</h2>
<p><img src="https://raw.githubusercontent.com/huilaaja/RedirectManager/master/images/redirect-manager-4.png" /></p>

<h2>Basic 404 redirect logic</h2>
<p><img src="https://raw.githubusercontent.com/huilaaja/RedirectManager/master/images/redirect-manager-5.png" /></p>