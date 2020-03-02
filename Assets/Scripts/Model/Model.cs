public class Model {

	public readonly string Id;
	public TimeChanges TimeChanges;

	public Model(string id, TimeChanges timeChanges) {
		TimeChanges = timeChanges;
		Id = id;
	}
}