using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public partial class Main : Node
{
	[Export] private PackedScene playerScene; // Asigna escena  de jugador en el editor
	//Jugador actual 

	private bool gamePaused=false;
	private bool gameOver = false;
	// pre cargamos los Obstáculos
	private PackedScene spikeScene = GD.Load<PackedScene>("res://Assets/Prefabs/spike.tscn");
	private PackedScene lavaScene = GD.Load<PackedScene>("res://Assets/Prefabs/lava.tscn");
	private PackedScene[] obstacles;
	private double spawnTimer = 0;
	//Intervalos en los que aparece los obstaculo
	private const double SPAWN_INTERVAL = 3.0;
	// Enemigos
	private PackedScene frogrosso = GD.Load<PackedScene>("res://Assets/Prefabs/frogrosso.tscn");
	private PackedScene eagearl = GD.Load<PackedScene>("res://Assets/Prefabs/eagearl.tscn");
	private PackedScene[] enemies;
	private double enemySpawnTimer = 0;
	private const double ENEMY_SPAWN_INTERVAL = 5.0; // Cada 5 segundos
	



	// Posiciones iniciales del jugador,la cámara Y suelo
	public static readonly Vector2 PLAYER_START_POS = new Vector2(70,491); //posicion inicial del jugador
	public static readonly Vector2 GROUND_INITIAL_POS = new Vector2(985,608); //posicion inicial del suelo
	public static readonly Vector2 CAM_START_OFFSET = new Vector2(576, 325); // Ajuste relativo al jugador
	public static  Vector2 screen_size= new Vector2(576, 325);//Tamaño de la pantalla

	//Puntuacion/Recorrido que lleva hecho el jugador 
	private float score=0f;
	// Velocidades del jugador
	private float speed;
	public const float START_SPEED = 700.0f;
	public const float MAX_SPEED = 900.0f;

	// Referencias a los nodos
	private TextureProgressBar progress;
	private CharacterBody2D player;
	private Player jugador;
	Jugador datosPlayer;
	private Camera2D camera;
	private StaticBody2D ground;
	private CanvasLayer menuOnGame;
	private CanvasLayer menuOnPause;
	
	
	
	public override void _Ready()
	{	//Obtener tamaño de la pantalla
		screen_size=DisplayServer.WindowGetSize();
		// Obtener referencias a los nodos
		progress = GetNode<TextureProgressBar>("OnGame/Progress/ProgressBar");
		menuOnGame = GetNode<CanvasLayer>("OnGame");
		menuOnPause = GetNode<CanvasLayer>("MenuPause");
		progress.MaxValue = 20000; // Valor maximo es decir donde acaba el nivel 
		progress.Value = 0;	
		//Cargamos jugador seleccionado 
		LoadPlayer();
		player = GetNode<CharacterBody2D>("Player");
		camera = GetNode<Camera2D>("Camera2D");
		ground = GetNode<StaticBody2D>("Ground");
		NewGame();
		obstacles = new PackedScene[] { spikeScene, lavaScene };
		enemies = new PackedScene[] { frogrosso, eagearl};
	}
	
	 private void LoadPlayer()
	{
		// Leer archivo JSON de personajes
		string path = "res://Assets/Resources/personajes.json";
		if (!FileAccess.FileExists(path))
		{
			GD.PrintErr("Archivo de personajes no encontrado.");
			return;
		}
		using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		string jsonText = file.GetAsText();

		List<Jugador> personajes = JsonSerializer.Deserialize<List<Jugador>>(jsonText);
		GD.Print("Cargando jugador...");
		// Buscar personaje seleccionado
		string selectedName = Global.namePlayer ?? "Vagabundo";
		datosPlayer = personajes.Find(p => p.name == selectedName);

		// Personaje
		playerScene = GD.Load<PackedScene>(Global.SelectedCharacter);
		player = playerScene.Instantiate<CharacterBody2D>();
		player.Name = "Player";
		AddChild(player);
		// Conectar señal de muerte
		if (player.HasSignal("PlayerDied"))
		{
			player.Connect("PlayerDied", new Callable(this, nameof(OnPlayerDeath)));
		}
	}
	
	public Jugador setDatosPersonaje()
	{
		return datosPlayer;
	}
	
	//Spawn aleatoria de enemigos
	public void SpawnRandomEnemy()
	{
		// Elegir aleatoriamente un enemigo
		var random = new Random();
		int index = random.Next(enemies.Length);
		var scene = enemies[index];

		// Instanciar el enemigo
		var enemy = scene.Instantiate<Node2D>();

		// Posición: justo fuera de la cámara, a la altura del suelo
		float spawnX = camera.Position.X + Main.screen_size.X + 100; // un poquito más lejos
		float spawnY = ground.Position.Y;
		spawnY -= 100;

		//Si es volador lo ubicamos un poco mas arriba 
		if (enemy.Name.ToString().ToLower().Contains("eagearl")) // ejemplo de Eagearl
		{
			 spawnY -= 200; // que aparezca volando más arriba
		}
		enemy.Position = new Vector2(spawnX, spawnY);
		GD.Print("Enemigo Spawneado: "+enemy.Name.ToString());
		AddChild(enemy);
	}
	//Spawn aleatoria de obstaculos
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
		float spawnY = ground.Position.Y+4f;

		// Si el nombre contiene "spike", subirlo un poco
		if (obstacle.Name.ToString().ToLower().Contains("spike"))
		{
			spawnY -= 100;
		}

		obstacle.Position = new Vector2(spawnX,spawnY);

		AddChild(obstacle);
	}

	public void NewGame()
	{
		// Posicionar al jugador en su punto de inicio
		player.Position = PLAYER_START_POS;
		player.Velocity = Vector2.Zero;
		progress.Value=0;
		// 🔹 Posicionar la cámara RELATIVAMENTE al jugador
		camera.Position =CAM_START_OFFSET;
		ground.Position =GROUND_INITIAL_POS;
	}

	  public override void _Process(double delta)
	  {			
		//Pausa el juego cuando se pulsa escape(En el caso del ordenador)
		  if (Input.IsActionJustPressed("pause_key"))
		  {
			menuOnPause.Visible=!gamePaused;
			gamePaused = !gamePaused;
			menuOnGame.Visible=!gamePaused;
		 }
		//Si la barra de nivel llega al maximo cambio escena 
		if(progress.Value==progress.MaxValue){
			GetTree().ChangeSceneToFile("res://Assets/Prefabs/main2.tscn");
			
		}
		if (gamePaused)
		return; // No procesamos nada si está en pausa
		
		//Suma por cada frame del juego 
		spawnTimer += delta;
		enemySpawnTimer += delta;
		//Si supera al intervalo  genera un obstaculo y reiniciamos el temporizador 
		if (spawnTimer >= SPAWN_INTERVAL)
		{
			SpawnRandomObstacle();
			spawnTimer = 0;
		}
			if (enemySpawnTimer >= ENEMY_SPAWN_INTERVAL)
		{
			SpawnRandomEnemy();
			enemySpawnTimer = 0;
		}
		speed = START_SPEED;

		// Mover player y cámara en X
		if(gameOver==true){
		player.Position += new Vector2(0 * (float)delta, 0);
		camera.Position += new Vector2(0 * (float)delta, 0);
		}else{
		player.Position += new Vector2(speed * (float)delta, 0);
		camera.Position += new Vector2(speed * (float)delta, 0);
		//Suma puntuacion
		score+=10;
		}
		//suma de progreso en el nivel 
		progress.Value=score;		
		foreach (Node child in GetChildren())
		{
			// Solo nos interesa si está en el grupo "Obstacle"
			if (child.IsInGroup("obstacle"))
			{
				//Convertimos a Node2D para poder acceder a su posición
				Node2D obstacle = (Node2D)child;

				// Si está más de 200px a la izquierda de la cámara, lo borramos
				if (obstacle.Position.X < camera.Position.X - 800)
				{
					obstacle.QueueFree();
					GD.Print("obstaculo liberado");
				}
				}else if (child.IsInGroup("Enemy"))
				{
					//Convertimos a Node2D para poder acceder a su posición
					Node2D enemy = (Node2D)child;

					// Si está más de 200px a la izquierda de la cámara, lo borramos
					if (enemy.Position.X < camera.Position.X - 800)
					{
						enemy.QueueFree();
						GD.Print("enemigo liberado");
					}
				}
		}
		// generacion de suelo unico de manera indefinida
		if (camera.Position.X - ground.Position.X > screen_size.X * 1.3)
		{
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
