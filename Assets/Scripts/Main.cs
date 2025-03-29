using Godot;
using System;

public partial class Main : Node
{
	// Posiciones iniciales del jugador y la cámara
	public static readonly Vector2 PLAYER_START_POS = new Vector2(-518, -58);
	public static readonly Vector2 CAM_START_OFFSET = new Vector2(0, -255); // Ajuste relativo al jugador

	// Velocidades
	private float speed;
	public const float START_SPEED = 100.0f;
	public const float MAX_SPEED = 250.0f;

	// Referencias a los nodos
	private CharacterBody2D player;
	private Camera2D camera;
	// private StaticBody2D ground;

	public override void _Ready()
	{
		// Obtener referencias a los nodos
		player = GetNode<CharacterBody2D>("Player");
		camera = GetNode<Camera2D>("Camera2D");
		// ground = GetNode<StaticBody2D>("ground");

		NewGame();
	}

	public void NewGame()
	{
		// Posicionar al jugador en su punto de inicio
		player.Position = PLAYER_START_POS;
		player.Velocity = Vector2.Zero;

		// 🔹 Posicionar la cámara RELATIVAMENTE al jugador
		camera.Position =CAM_START_OFFSET;
		// ground.Position = Vector2.Zero;
	}

	  public override void _Process(double delta)
	{
		speed = START_SPEED;

		// Mover dino y cámara en X
		player.Position += new Vector2(speed * (float)delta, 0);
		camera.Position += new Vector2(speed * (float)delta, 0);
	}
}
