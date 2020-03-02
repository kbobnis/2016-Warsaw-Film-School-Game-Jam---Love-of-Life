public class ScheduledSituation {
	public readonly int Duration;
	public readonly int From;
	public readonly Situation Situation;
	public readonly bool Permament;

	public ScheduledSituation(int from, int duration, Situation situation, bool permament) {
		From = from;
		Duration = duration;
		Situation = situation;
		Permament = permament;
	}

	public override string ToString() {
		return Situation.Id + " from " + From + " duration " + Duration;
	}
}