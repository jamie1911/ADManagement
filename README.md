# ADManagement

<p>I developed this application in my down time to get more familiar with AngularJs. However, it has a purpose outside of this. I also created it so people can use this to expose some key AD attributes to other AD users for editing. They can be responsible for updating Employee Photos and Information without getting exposed to Active Directory Users and Computers or any other low level tool.</p>
<p>The application is written primarily in HTML5, C#, and AngularJS</p>
<p>Since this is a complete rewrite of my previous application I built for a company, it is still in development and I am working hard to get it to a stable version.</p>

<h3>To do list</h3>
<ul>
<li>Get alert queue finished</li>
<li>Finish impersonation</li>
<li>Finish audit trail</li>
<li>Various bug fixes</li>
<li>Cleanup code</li>
<li>Finish Install Document</li>
</ul>

<h3>How it works</h3>
<p>Using Kerberos impersonation, the user connects to active directory with his/her credentials and performs the field updates.</p>
<p>After creating a "ADManagement Users" AD group, you grant it granular access at the OU level to allow write access to certain AD attributes. This allows you to control who can update the fields you approve</p>

<h3>Requirements</h3>
<ul>
<li>Server 2008R2+ with IIS</li>
<li>.NET 4.5</li>
<li>MVC 4</li>
</ul>

<h3>Quick Demo</h3>
<h5>Main View</h5>
<img src="https://github.com/jamie1911/ADManagement/blob/master/doc/main.jpg" />
<h5>Search View</h5>
<img src="https://github.com/jamie1911/ADManagement/blob/master/doc/search.jpg" />
<h5>User Profile View</h5>
<img src="https://github.com/jamie1911/ADManagement/blob/master/doc/userprofile.jpg" />
<h5>Group View</h5>
<img src="https://github.com/jamie1911/ADManagement/blob/master/doc/groupprofile.jpg" />
<h5>Photo Update</h5>
<img src="https://github.com/jamie1911/ADManagement/blob/master/doc/photo.jpg" />
<br>
