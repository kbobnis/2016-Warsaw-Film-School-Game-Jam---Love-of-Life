
using System.Collections.Generic;

public class TimeChanges {
	
	public readonly float NormalSpeed;
	public readonly float FasterSpeed;
	public readonly List<Change> Changes;

	public TimeChanges(float normalSpeed, float fasterSpeed, List<Change> timeChanges) {
		NormalSpeed = normalSpeed;
		FasterSpeed = fasterSpeed;
		Changes = timeChanges;
	}
}
