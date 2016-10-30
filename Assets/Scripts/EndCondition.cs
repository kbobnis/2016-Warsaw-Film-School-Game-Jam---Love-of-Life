
internal abstract class EndCondition {
	private GameState GameState;

	public EndCondition(GameState gameState) {
		GameState = gameState;
	}

	internal abstract string GetText();

	internal class Lose : EndCondition {
		private Parameter Parameter;

		public Lose(Parameter parameter, GameState gameState) : base(gameState){
			Parameter = parameter;
		}

		internal override string GetText() {
			return "Przegrałeś w dniu " + GameState.DayNumber + ", bo wartośc parametru " + Parameter.Text + " spadła poniżej zera.";
		}
	}

	internal class Win : EndCondition {
		private Plot.Element PlotElement;

		public Win(Plot.Element plotElement, GameState gameState) : base(gameState)  {
			PlotElement = plotElement;
		}

		internal override string GetText() {
			return "Wygrałeś, bo skończyłeś zadanie: " + PlotElement.Text + " w czasie " + GameState.DayNumber + "dni, " + (int)GameState.HourOfDay + " godzin.";
		}
	}
}