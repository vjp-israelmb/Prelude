using Godot;
using System;

public partial class Player : CharacterBody2D
{
	//audios del jugador 
	private AudioStreamPlayer audioJump;
	private AudioStreamPlayer audioHit;
	private AudioStreamPlayer audioDeath;
	
	//Para obtener la animacion del corazon
	private Node2D heartBar;
	private AnimatedSprite2D heart;
	//Booleano para cuando es golepado
	private bool isHit = false;
	[Export]
	public int GRAVITY { get; private set; } = 4200;
	
	[Export]
	public int JUMP_SPEED { get; private set; } = 1800;

	private AnimatedSprite2D anim;
	private AnimationPlayer animPlayer;
	public String name;
	public int hp;
	public int armor;
	public bool isDead = false;
	//Declaracion  de señal para la muerte del jugador
	[Signal]
	public delegate void PlayerDiedEventHandler();
	
	//Funcion para hacer las animaciones de la vida del jugador 
	public void UpdateHeart(int currentHp)
	{
		if (heart == null)
		{
			GD.PrintErr("Heart no está asignado.");
			return;
		}

		string animName = "";

		if (currentHp >= 6)
			animName = "Full";
		else if (currentHp == 1)
			animName = "Lose4";
		else if (currentHp <= 0)
			animName = "Lose6";
		else
			animName = $"Lose{6 - currentHp}";

		if (heart.SpriteFrames.HasAnimation(animName))
		{
			heart.Play(animName);
			GD.Print($"Reproduciendo animación: {animName}");
		}
		else
		{
			GD.PrintErr($"La animación '{animName}' no existe en el AnimatedSprite2D.");
		}
	}
	
	public override void _Ready()
	{
		

		try {
			audioJump = GetNode<AudioStreamPlayer>("AudioJump");
			audioHit = GetNode<AudioStreamPlayer>("AudioHit");
			audioDeath = GetNode<AudioStreamPlayer>("AudioDeath");
			anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
			heartBar = GetNode<Node2D>("../OnGame/HeartBar");
			heart = heartBar.GetNode<AnimatedSprite2D>("Heart");
		} catch (Exception e) {
			GD.PrintErr("Error accediendo a nodos: ", e.Message);
		}
		UpdateHeart(hp);
		if (Global.SelectedCharacter == "res://Assets/Prefabs/Knigth.tscn")
		{
			armor = 6;
			GD.Print("Caballero detectado. Armadura inicial: " + armor);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		//GD.Print($"Estado isHit: {isHit} | Animación: {anim.Animation} | Colisión: {GetNode<CollisionShape2D>("CollisionShape2D").Disabled}");
		if (!isDead && isHit) return; // si está golpeado pero no muerto, no hacer animaciones normales
		//Si está muerto, solo aplicar gravedad
		if (isDead)
		{
			Velocity = new Vector2(0, Velocity.Y + (float)(GRAVITY * delta)); 
			MoveAndSlide();
			return;
		}
		Velocity = new Vector2(Velocity.X, Velocity.Y + (float)(GRAVITY * delta));
		
		if (IsOnFloor())
		{
			if(Velocity.Y>0f)
			{
			if (Input.IsActionJustPressed("jump_key"))
			{
				Velocity = new Vector2(Velocity.X, -JUMP_SPEED);
				audioJump.Play();
			}
			else
			{
				try{
					anim.Play("Moving");
				}catch(NullReferenceException x){
				}
			}
			}
			else
			{
				try{
					anim.Play("Idle");
				}catch(NullReferenceException x){
				}
				
			}
		}
		else
		{
			try{
			anim.Play("Jump");}catch(NullReferenceException x){	
			}
		}

		MoveAndSlide();
	}

	public async void Hit()
	{
	if (isHit) return;
	isHit = true;
	try
	{
		// Detener y reproducir animación (usando variable existente)
		anim.Stop();
		anim.Play("Hit");
		audioHit.Play();

		// Calcular duración exacta (frame count * tiempo por frame)
		double hitDuration = (anim.SpriteFrames.GetFrameCount("Hit") / anim.SpriteFrames.GetAnimationSpeed("Hit"))/anim.SpeedScale;
		
		// Esperar animación + margen de seguridad
		await ToSignal(GetTree().CreateTimer(hitDuration), "timeout");

		// Reset seguro (evita que quede en estado intermedio)
		if (anim.IsPlaying() && anim.Animation == "Hit")
		{
			anim.Play("Moving");
		}
	}
	finally
	{
		isHit = false;
	}
		
		GD.Print("Jugador golpeado");
	if (armor > 0){
		armor -= 1;
		GD.Print("Armor restante: " + armor);
	}else{
		hp -= 1;
		if (heart != null) UpdateHeart(hp);
		GD.Print("HP restante: " + hp);
	}
	
	if (hp <= 0) {
			Die();
	}

}

	private void Die()
	{
		isDead = true;

		// Detener movimiento
		GRAVITY = 0;
		SetProcess(false);
		SetPhysicsProcess(false);
		GD.Print("El jugador ha muerto");
		audioDeath.Play();
		anim.Play("Death");
		// Esperar a que termine la animación para reiniciar
		EmitSignal(SignalName.PlayerDied);
	}
}
