namespace Sp.Portal.Acceptance
{
	using Sp.Portal.Acceptance.Wrappers;
	using Xunit;
	using Xunit.Extensions;

	public static class LandingPageFacts
	{
		[Theory, PortalData]
		public static void LandingPageShouldReturnHtml( SpPortalConfiguration spPortalConfiguration)
		{
			//TODO
			Assert.True( true );
		}
	}
}
