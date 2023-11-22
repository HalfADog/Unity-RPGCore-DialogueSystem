namespace RPGCore.Base
{
	public class Singleton<T> where T : new()
	{
		private static T minstance;

		public static T Instance
		{
			get
			{
				if (minstance == null)
				{
					minstance = new T();
				}
				return minstance;
			}
		}
	}
}
