
internal abstract class EndCondition {
	private GameState GameState;

	internal abstract string GetText();

	internal class Lose : EndCondition {
		private Parameter Parameter;

		public Lose(Parameter parameter, GameState gameState) {
			Parameter = parameter;
			GameState = gameState;
		}

		internal override string GetText() {
			return "Przegrałeś w dniu " + GameState.DayNumber + ", bo wartośc parametru " + Parameter.Text + " spadła poniżej zera.";
		}
	}

	internal class Win : EndCondition {
		private Plot.Element PlotElement;

		public Win(Plot.Element plotElement) {
			PlotElement = plotElement;
		}

		internal override string GetText() {
			return "Wygrałeś, bo skończyłeś zadanie: " + PlotElement.Text;
		}
	}
}