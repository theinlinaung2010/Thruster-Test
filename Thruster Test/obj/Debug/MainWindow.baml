   M S B A M L   `   `   `  ����U  QLiveCharts.Wpf, Version=0.9.7.0, Culture=neutral, PublicKeyToken=0bc1f845d1ebb8dfG4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.WpfLiveCharts.Wpf  5   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5   �    Thruster Test   Thruster_Test.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Dlvp4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.Wpf     5   	   U QMaterialDesignThemes.Wpf, Version=2.3.0.823, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      :d2http://schemas.microsoft.com/expression/blend/2008  5   	   Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  6F   3System&clr-namespace:System;assembly=mscorlib  6�   -   5          � Title$  
Main Panel��5   	   $��589.51��6   $��1087.937��6,    � WindowStartupLocation$ CenterScreen=�6=      MaterialDesignBody#���   5        MaterialDesignPaper#���  5	   	   $��Medium"�5
   	   $��14(�5   	   $��Roboto)�5       � WindowState$ 	Maximized=�6    � Icon$ Propeller.ico��6]    � 	Resources 5        NormalFontSize'  !MaterialDesignThemes.Wpf.PackIcon  	StartIcon�  �ControlzEx.PackIconBase`1[[MaterialDesignThemes.Wpf.PackIconKind, MaterialDesignThemes.Wpf, Version=2.3.0.823, Culture=neutral, PublicKeyToken=null]]   Kind  StopIcon��%�   &	       &	      &	 T     X�5   
   16Q�63     5   
   $
 Play=�64   $��30��6@   $��30��6L     5   
   $
 Stop=�63   $��30��6?   $��30��6K   5      .��5      �  � RowDefinitions 5   
   �� 5      $
��179* �6   �� 5      $
��211* �6   �� 5      $	��90* �6   �� 5      $	��79* �6    � ColumnDefinitions 5   
   �� 5      $
��823* �6   �� 5      $
��120* �6   �� 5      $
��120* �6   .+�5   
   #  MaterialDesignThemes.Wpf.Card  $��15q�6   $��5,5,3,5q�6+   $��Stretch=�6<   $��4��6X   .��5       � $��0,0,-12,-4.4q�6   .+�5!      #   LiveCharts.Wpf.CartesianChart  -   5          Name$ mChart��5!   %   $��	0,0,10,52q�65   &    LiveCharts.Wpf.Charts.Base.Chart	   AnimationsSpeed$	 0:0:0.3h�6H   
   DisableAnimations
 . 
   Pan$ X=�6{      Zoom$ X=�6�      Series 5#         LiveCharts.Wpf.LineSeries     AxisY 5&         LiveCharts.Wpf.Axis     
FontFamily$ Roboto)�6#      MinRange$ 5Q�67      MaxRange$ 50Q�6D      Title$ Current (A)��6R      Unit$	 0.1Q�6f      BarUnit$ 1Q�6q      MinValue$ 0Q�6}      Position$ RightTop=�6�     CurrentBrush   
Foreground# �  6�      
ShowLabels .    	Separator 5'         LiveCharts.Wpf.Separator  5(   "      StrokeThickness$ 0Q�60   5*        5+      $ Roboto)�6#   $ Voltage (V)��67   $ 1Q�6K   $ 15Q�6W   $ 30Q�6e   $ 0Q�6s   $ 1Q�6�   $ RightTop=�6�     VoltageBrush# �  6�    5,        5-   "   $ 0Q�60   5/        50      $ Roboto)�6#   $ Throttle (%)��67   $
 -100Q�6L      MaxValue$	 100Q�6\   $ 1Q�6k   $ 30Q�6w   $	 200Q�6�   $ 1Q�6�     ThrottleBrush# �  6�    51        52   "   $ 0Q�60   54        55      $ 1Q�6#   $ 2Q�6/   $ 50Q�6<   $ Thrust (kg)��6J   $ 1Q�6^     ThrustBrush# �  6g      AxisX 59        $ Time (s)��6#   $ Roboto)�64      DisableAnimations . $ 15Q�6a   $ 0Q�6o   $ 10Q�6|   $ 20Q�6�   5<      A� 5>      $��20,0,0,0q�6    A� Columns$ 5��60    A� Rows$ 1��6<     J� RenderTransformOrigin$  0.5,0.5&�6E   $��52��6e   $��Bottom=�6q   .+�5?      � $��	0,10,5,10q�6   .+�5@      �� -   5        ! ��Name$! 
CBThrottle��5@   #   $
��Left=�67   $��45��6R    $	  MaterialDesignSwitchToggleButton#��� 	 6]   $��0q�6�   " d� 	IsChecked$
" TrueY�6�   �� 5A      $��Throttle��6    $��Center=�63   $��45,0,0,0q�6U   #���  6g   #���  6�   5B      � 5C      $
��0,10q�6   .+�5D      �� -   5        $! CBThrust��5D   #   $
��Left=�65   $��45.903��6P   #��� 	 6_   $��0q�6�   $
" TrueY�6�   �� 5E      $��Thrust��6    $��Center=�61   $��46,0,0,0q�6S   #���  6e   #���  6�   5F      � 5G      $
��0,10q�6   .+�5H      �� -   5        $! 	CBCurrent��5H   #   $
��Left=�66   $��45.903��6Q   #��� 	 6`   $��0q�6�   $
" TrueY�6�   �� 5I      $��Current��6    $��Center=�62   $��46,0,0,0q�6T   #���  6f   #���  6�   5J      � 5K      $
��0,10q�6   .+�5L      �� -   5        $! CBrpm��5L   #   $
��Left=�62   $��45.903��6M   #��� 	 6\   $��0q�6�   6�   �� 5M      $	��RPM��6    $��Center=�6.   $��46,0,0,0q�6P   #���  6b    
 RpmBrush#��� 
 6�   5N      � 5O      $
��0,10q�6   .+�5P      �� -   5        $! 	CBVoltage��5P   #   $
��Left=�66   $��45.903��6Q   #��� 	 6`   $��0q�6�   $
" TrueY�6�   �� 5Q      $��Voltage��6    $��Center=�62   $��46,0,0,0q�6T   #���  6f   #���  6�   5R      5S      5T      5V        5W   
   $��15q�6   $��3,3,6,3q�6+   $��Stretch=�6<   $��2��6X   $��1��6l   $��1��6|   .��5X      �� -   5        # ��Name$# CmdTB��5X      $ �� TextWrapping$
$ Wrap=�6%   % �� AcceptsReturn% . "& �� VerticalScrollBarVisibility$
& Auto=�6N   )	  #MaterialDesignThemes.Wpf.HintAssist' 	  Hint$' Type Commands Here��6q   $��0,0,0.4,0.4q�6�   $	��Top=�6�   $��16(�6�   5Y      �� 5Z   
   ���	#4CFFF620$��14.4,18.8,14.6,192.4q�6%   $��1��6C   $��1��6S   $��2��6`   ( J� IsHitTestVisible( .  $y�Hidden=�6�   �� 5[   
   -	   5        ) ��Name$) BtnStart��5[      $��1��6#   $��0,6.2,20,10q�63   $��Bottom=�6H   $��60��6c    & "MaterialDesignFloatingActionButton#���  6o   $��60��6�   $��Right=�6�   $  1.148,0.414&�6�   $��3��6�   #���  6  * � ToolTip$* Start��6A    5\   
   $��0q�6   $��3,5,6,3q�6*   $��Stretch=�6;   $��2��6W   $��1��6k   .��5]      � $��10q�6    5^      �� 5_      $
��121* �6'   �� 5`      $	��70* �6'    5b      �� 5c      $	��20* �6$   �� 5d      $	��20* �6$   �� 5e      $	��20* �6$   �� 5f      $	��20* �6$   �� 5g      $	��20* �6$   .+�5i      ��    MaterialDesignTitleTextBlock#���  6   $
��Wrap=�6S   $��Throttle (%)��6g   $��0q�6{   ���	#4C00EA00+ �� Padding$+ 5q�6�   #���  6�   �� 5j      #���  6   $
��Wrap=�6S   $��Thrust (kg)��6g   $��0q�6z   ���	#4C0A58FF$+ 5q�6�   #���  6�   $��1��6�   �� 5k      #���  6   $
��Wrap=�6S   $��Current (A)��6g   $��0q�6z   $��2��6�   ���	#4CFF0000$+ 5q�6�   #���  6�   �� 5l      -
   5        , ��Name$, 
TBThrottle��5l      #���  60   $
��Wrap=�6g   $	��0.0��6{   $��0q�6�   $��RightToLeft=�6�   ���	#4C00EA00$+ 5q�6�   #���  6�   $  0.5,0.5&�6�   $��1��6  $
��Bold"�6,  z�5m      V� 5n      .~�5o      �� �� 5p      �� 5q      T� 5r      - T� X$- 0.561Q�61   5s      5u      �� 5v      -   5        $, TBThrust��5v      #���  6.   $
��Wrap=�6e   $	��0.0��6y   $��0q�6�   $��RightToLeft=�6�   $��1��6�   ���	#4C0A58FF$+ 5q�6�   $��1��6�   #���  6�   $
��Bold"�6  �� 5w      -   5        $, TBrpm��5w      #���  6+   $
��Wrap=�6b   $	��0.0��6v   $��0q�6�   $��RightToLeft=�6�   $��3��6�   ���	#7FFFC0CB$+ 5q�6�   $��1��6�   #���  6�   $
��Bold"�6  �� 5x      #���  6   $
��Wrap=�6S   $	��RPM��6g   $��0q�6r   $��3��6}   ���	#7FFFC0CB$+ 5q�6�   #���  6�   �� 5y      -   5        $, 	TBCurrent��5y      #���  6/   $
��Wrap=�6f   $	��0.0��6z   $��0q�6�   $��RightToLeft=�6�   $��2��6�   ���	#4CFF0000$+ 5q�6�   #���  6�   $��1��6  $
��Bold"�6  �� 5z      -   5        $, 	TBVoltage��5z      #���  6/   $
��Wrap=�6f   $	��0.0��6z   $��0q�6�   $��RightToLeft=�6�   $��4��6�   ���	#4CFFBF00$+ 5q�6�   #���  6�   $��1��6  $
��Bold"�6  �� 5{      #���  6   $
��Wrap=�6S   $��Voltage (V)��6g   $��0q�6z   $��4��6�   ���	#4CFFBF00$+ 5q�6�   #���  6�   5|      5}        5~   
   $��15q�6   $��3,3,6,3q�6+   $��Stretch=�6<   $��2��6X   $��2��6l   $��1��6y   .��5      �� -   5        $# TBNotes��5      $
$ Wrap=�6'   % . $
& Auto=�6P   $' Notes��6s   $��	0,0,0.4,0q�6�   $��16(�6�   $	��Top=�6�   5�      �� 5�   
   -   5        $) BtnData��5�      $��2��6"   $��20,6.2,0,10q�62   $��Bottom=�6G   $��60��6b    , (MaterialDesignFloatingActionAccentButton#���  6n   $��60��6�   $
��Left=�6�   $  1.148,0.414&�6�   $��3��6�   $* Analysis��6  .��5�        $ Database=�6&   $��30��66   $��30��6B   5�      '
  !MaterialDesignThemes.Wpf.Snackbar
  5�   
   -   5        . 
 Name$. 	mSnackBar��5�   /   $��3��6"   6/   5�      5�      