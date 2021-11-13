using AutoFixture;


namespace Keebox.Common.IntegrationTests.Helpers
{
	public static class Creator
	{
		public static string CreateStringWithMaxLength(int length)
		{
			return string.Join(string.Empty, _fixture.CreateMany<char>(length));
		}

		private static readonly Fixture _fixture = new();
	}
}