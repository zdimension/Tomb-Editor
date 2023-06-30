﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DarkUI.WPF.Demo;

public class Person
{
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public int Age { get; set; }
	public string Country { get; set; }
}

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();

		OriginalPreviewHost.Child = new OriginalComparison();

		// Generate random person records
		var people = new List<Person>();
		var random = new Random();

		for (int i = 0; i < 10; i++)
		{
			people.Add(new Person
			{
				FirstName = "First " + i,
				LastName = "Last " + i,
				Age = random.Next(18, 65),
				Country = "Country " + i
			});
		}

		dataGrid.ItemsSource = people;
	}
}
