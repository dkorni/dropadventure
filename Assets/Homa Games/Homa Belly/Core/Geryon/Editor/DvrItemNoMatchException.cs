using System;

namespace HomaGames.HomaBelly
{
    public class DvrItemNoMatchException : Exception
    {
        public DvrItemNoMatchException() : base("No DVR found in remote configuration (error 400). " +
                                                 "Please check your configuration exists in Hom Lab. Otherwise please reach our Help Center.")
        {
            
        }
    }
}
