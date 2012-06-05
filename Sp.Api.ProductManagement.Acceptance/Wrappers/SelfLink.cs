namespace Sp.Api.ProductManagement.Acceptance.Wrappers
{
	using System;

	public class SelfLink
	{
		public Link self { get; set; }

		public class Link
		{
			public string href { get; set; }

			public Uri AsRelativeUri()
			{
				return new Uri( href, UriKind.Relative );
			}
		}
	}
}