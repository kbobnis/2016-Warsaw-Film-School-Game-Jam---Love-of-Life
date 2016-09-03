internal class ScheduledSituation {
	public readonly int Duration;
	public readonly float From;
	public readonly Situation Situation;
	public readonly bool Permament;

	public ScheduledSituation(float from, int duration, Situation situation, bool permament) {
		From = from;
		Duration = duration;
		Situation = situation;
		Permament = permament;
	}

	public override string ToString() {
		return Situation.Id + " from " + From + " duration " + Duration;
	}
}