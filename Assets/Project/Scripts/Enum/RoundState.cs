namespace ReGaSLZR.Dare.Enum
{

    public enum RoundState 
    {
        NotStarted,
        CountdownToStart,
        /// <summary>
        /// Round countdown is ongoing.
        /// </summary>
        InPlay,
        /// <summary>
        /// Round countdown is on pause.
        /// </summary>
        Paused,
        /// <summary>
        /// Round countdown finished.
        /// </summary>
        Finished,
        /// <summary>
        /// No more succeeding rounds.
        /// </summary>
        GameOver,
    }

}