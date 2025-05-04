using Godot;
using System;
using System.Collections.Generic;

public partial class HeartBar : Node
{
	// Referencias a cada corazón
	private List<AnimatedSprite2D> hearts = new();

	public override void _Ready()
	{
		// Buscar todos los corazones por nombre fijo
		for (int i = 1; i <= 6; i++)
		{
			string name = i == 1 ? "Heart" : $"Heart{i}";
			var heart = GetNode<AnimatedSprite2D>($"../{name}");
			hearts.Add(heart);
		}
	}
	public void UpdateHearts(int currentHp)
	{
		for (int i = 0; i < hearts.Count; i++)
		{
			if (i < currentHp)
			{
				hearts[i].Play("Gain");
				CallDeferred(nameof(SetAnimationAfter), hearts[i], "Full");
			}
			else
			{
				hearts[i].Play("Lose");
				CallDeferred(nameof(SetAnimationAfter), hearts[i], "Empty");
			}
		}
	}

	private async void SetAnimationAfter(AnimatedSprite2D heart, string animName)
	{
		await ToSignal(heart, "animation_finished");
		heart.Play(animName);
	}
}
