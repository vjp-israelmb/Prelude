using Godot;
using System;

public partial class Player : CharacterBody2D
{
	//Booleano para cuando es golepado
	private bool isHit = false;
	[Export]
	public int GRAVITY { get; private set; } = 4200;
	[Export]
	public int JUMP_SPEED { get; private set; } = 1800;

	private AnimatedSprite2D anim;
	public int hp = 100;
	public int armor = 0;
	public bool isDead = false;
	//Declaracion  de señal para la muerte del jugador
	[Signal]
	public delegate void PlayerDiedEventHandler();

	public override void _Ready()
	{
		anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (Name.ToString().ToLower().Contains("knight"))
		{
			armor = 50;
			GD.Print("🛡️ Caballero detectado. Armadura inicial: " + armor);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (isDead || isHit) return; // No moverse si está muerto

		Velocity = new Vector2(Velocity.X, Velocity.Y + (float)(GRAVITY * delta));

		if (IsOnFloor())
		{
			if (Input.IsActionJustPressed("jump_key"))
			{
				Velocity = new Vector2(Velocity.X, -JUMP_SPEED);
			}
			else
			{
				anim.Play("Moving");
			}
		}
		else
		{
			anim.Play("Jump");
		}

		MoveAndSlide();
	}

	public void Hit()
	{
		if (isDead || isHit) return;
		isHit=true;
		anim.Play("Hit");
		anim.Play("Hit");
		// Espera a que termine la animación antes de continuar
		GetTree().CreateTimer(0.4).Timeout += () =>
		{
			isHit = false;
		};
		GD.Print("💥 Jugador golpeado");
		if (armor>0){
			armor-=25;
		}else{
			hp -= 25;
		}
		if (armor>0){
			GD.Print("Armor restante: " + armor);
		}else{
			GD.Print("HP restante: " + hp);
		}
		if (hp <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		isDead = true;
		GD.Print("☠️ El jugador ha muerto");

		anim.Play("Death");

		// Esperar a que termine la animación para reiniciar
		anim.AnimationFinished += OnDeathAnimationFinished;
	}

	private void OnDeathAnimationFinished()
	{
		if (anim.Animation == "Death")
		{
			EmitSignal(SignalName.PlayerDied); //Avisamos al Main
		}
	}
}
