The Item Url Helper is has two main components

- Copy Item Url
	- This component adds a menu item to the context menu (right clicking the content tree) that returns a url for 
	  the selected an item for each of the presentation layers that have been set for that item.

	- Install the ItemUrlHelper_CopyItemUrl.zip package to your Sitecore instance.
		- This will install a ItemUrlHelper.config file as well as a Context Menu item in the core database.

- Item Resolver by Url
	- This component has it hooks in the search pipeline and tries to return an item from a passed url.  The search 
	  is located at the top of the content tree.
	  
	- Add this line to the search pipeline below the ID resolver.
		- <processor type="Velir.SitecoreLibrary.Modules.ItemUrlHelper.CustomSitecore.Pipeline.ItemResolverByUrl, Velir.SitecoreLibrary.Modules.ItemUrlHelper" />