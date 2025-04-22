using Godot;
using System;

public partial class Main : Node
{
	private bool gameOver = false;
	// Obstáculos
	private PackedScene spikeScene = GD.Load<PackedScene>("res://Assets/Prefabs/spike.tscn");
	private PackedScene lavaScene = GD.Load<PackedScene>("res://Assets/Prefabs/lava.tscn");
	private PackedScene[] obstacles;
	private double spawnTimer = 0;
	private const double SPAWN_INTERVAL = 3.0;
	



	// Posiciones iniciales del jugador,la cámara Y suelo
	public static readonly Vector2 PLAYER_START_POS = new Vector2(70,491); //posicion inicial del jugador
	public static readonly Vector2 GROUND_INITIAL_POS = new Vector2(962,620); 
	public static readonly Vector2 CAM_START_OFFSET = new Vector2(576, 325); // Ajuste relativo al jugador
	public static  Vector2 screen_size= new Vector2(576, 325);

	// Velocidades
	private float speed;
	public const float START_SPEED = 500.0f;
	public const float MAX_SPEED = 650.0f;

	// Referencias a los nodos
	private CharacterBody2D player;
	private Camera2D camera;
	private StaticBody2D ground;
	
	
	
	public override void _Ready()
	{	//Obtener tamaño de la pantalla
		screen_size=DisplayServer.WindowGetSize();
		
		// Obtener referencias a los nodos
		player = GetNode<CharacterBody2D>("Player");
		camera = GetNode<Camera2D>("Camera2D");
		ground = GetNode<StaticBody2D>("Ground");
		
		//Conectamos la señal de muerte
		if (player.HasSignal("PlayerDied")) 
		{
			player.Connect("PlayerDied", new Callable(this, nameof(OnPlayerDeath)));
		}	
		
		NewGame();
		obstacles = new PackedScene[] { spikeScene, lavaScene };
	}


	public void SpawnRandomObstacle()
	{
		// Elegir aleatoriamente un obstáculo
		var random = new Random();
		int index = random.Next(obstacles.Length);
		var scene = obstacles[index];

		// Instanciar el obstáculo
		var obstacle = scene.Instantiate<Node2D>();

		// Posición: justo fuera de la cámara, a la altura del suelo
		float spawnX = camera.Position.X + Main.screen_size.X + 100;
		float spawnY = ground.Position.Y;

		// Si el nombre contiene "spike", subirlo un poco
		if (obstacle.Name.ToString().ToLower().Contains("spike"))
		{
			spawnY -= 100; // súbelo 40 px o lo que necesites
		}

		obstacle.Position = new Vector2(spawnX, spawnY);

		AddChild(obstacle);
	}

	public void NewGame()
	{
		// Posicionar al jugador en su punto de inicio
		player.Position = PLAYER_START_POS;
		player.Velocity = Vector2.Zero;

		// 🔹 Posicionar la cámara RELATIVAMENTE al jugador
		camera.Position =CAM_START_OFFSET;
		ground.Position =GROUND_INITIAL_POS;
	}

	  public override void _Process(double delta)
	{	
		//Si es juego acabado no pasa y no avanza la camara ni el jugador 
		if (gameOver)
		return;
		//Suma por cada frame del juego 
		spawnTimer += delta;
		//Si supera al intervalo  genera un obstaculo y reiniciamos el temporizador 
		if (spawnTimer >= SPAWN_INTERVAL)
		{
			SpawnRandomObstacle();
			spawnTimer = 0;
		}
		
		speed = START_SPEED;
		// Mover player y cámara en X
		player.Position += new Vector2(speed * (float)delta, 0);
		camera.Position += new Vector2(speed * (float)delta, 0);
		foreach (Node child in GetChildren())
		{
		// Solo nos interesa si está en el grupo "Obstacle"
		if (child.IsInGroup("obstacle"))
		{
			//Convertimos a Node2D para poder acceder a su posición
			Node2D obstacle = (Node2D)child;

			// 📏 Si está más de 200px a la izquierda de la cámara, lo borramos
			if (obstacle.Position.X < camera.Position.X - 800)
			{
				obstacle.QueueFree();
				GD.Print("obstaculo liberado");
			}
		}
		}
		// generacion de suelo unico de manera indefinida
		if (camera.Position.X - ground.Position.X > screen_size.X * 1.3){
			 GD.Print("Reposicionando el suelo");
			//Creo una variable nueva y se lo asigno al suelo actual 
			Vector2 newGroundPos = ground.Position;
			newGroundPos.X += screen_size.X;
			ground.Position = newGroundPos;
		}
	}
	//Muerte del jugador 
	private void OnPlayerDeath()
	{
		gameOver = true;
		// Espera 1.5 segundos para que la animación se vea
		GetTree().CreateTimer(1.5).Timeout += () =>
		{
			GetTree().ReloadCurrentScene();
		};
	}
}
