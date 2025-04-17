using Godot;
using System;

public partial class Player : CharacterBody2D
{
	// Gravedad del juego para que el jugador no flote al saltar
	[Export]
	public int GRAVITY { get; private set; } = 4200;
	private AnimatedSprite2D anim;

	// En lugar de un nodo, referenciamos una escena (tscn)
	//[Export]
	//public PackedScene ClaseScene { get; private set; }
	//private CharacterBody2D claseInstance;

	// Velocidad de salto
	[Export]
	public int JUMP_SPEED { get; private set; } = 1800;

	 public override void _Ready()
	{
		
		anim=GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		//if (ClaseScene != null)
		//{
			//// Instancia la escena y la agrega como hijo de Player
			//claseInstance = ClaseScene.Instantiate<CharacterBody2D>();
			//AddChild(claseInstance);
//
			//// Asegura que herede la posición de Player
			//claseInstance.Position = Vector2.Zero;
		//}
	}

	public Vector2 ScreenSize;

	// Movimiento del personaje
	public override void _PhysicsProcess(double delta)
	{
		// Si está en estado de golpe, reproducir hit y salir
		if (isHit)
		{
			hitTimer -= delta;
			if (hitTimer <= 0)
				isHit = false;
			return;
		}

		// Aplicar gravedad
		Velocity = new Vector2(Velocity.X, Velocity.Y + (float)(GRAVITY * delta));

		// Animaciones normales
		if (IsOnFloor())
		{
			if(Input.IsActionJustPressed("jump_key")){
				Velocity = new Vector2(Velocity.X, -JUMP_SPEED);
			}else{
				anim.Play("Moving");
			}
		}
		else
		{
			anim.Play("Jump");
		}
		// Aplicar el movimiento
		MoveAndSlide();
	}
	
	//variable para si es golpeado
	private bool isHit = false;
	//tiempo que hace la animacion
	private double hitTimer = 0;
	private const double HIT_DURATION = 0.5; // medio segundo

	public void Hit()
	{
		GD.Print("Jugador recibió golpe de obstáculo");
		isHit = true;
		hitTimer = HIT_DURATION;
		anim.Play("Hit");
	}
}
