   M S B A M L   `   `   `  ����J  FUpRevFileOpener, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:UpRevFileOpenerUpRevFileOpener  5   	       UpRevFileOpener.Login   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:UpRevFileOpener     5   	     � WindowStartupLocation$  CenterScreen=�5   	    � AllowsTransparency . 
������  � ShowInTaskbar .   � WindowStyle$
 None=�5   	    � SizeToContent$ WidthAndHeight=�5   	   $	��350��5   	   $	��400��6    +� FocusedElement 5   	   ��  �� ElementName$ 
productKey��.��5      � .+�5   
   ��  �� CornerRadius$ 10��6   
���  ��$��3q�5   	   
���   �$��24q�5   	    �� Padding$ 4q�5   	   	 J� Effect	 5      3  -System.Windows.Media.Effects.DropShadowEffect  5      
   Color$

 Gray��6#      Opacity$	 .50Q�5         ShadowDepth$ 8Q�5      ._�5      �  � RowDefinitions 5      �� 5      $	��100 �6$   �� 5       $	��125 �6$   �� 5!      $��75 �6$   .+�5#      � $��0��6    � ColumnDefinitions 5$      �� 5%      $	��150 �6+   �� 5&      $��* �6+    5(      �� 5)      $	��100 �6(   .+�5+      �� $��0��6!   $��0��5,   !   .+�5-      �� -   5        $��imgKey��5-       $��8q�5.       $,��&/Images/uprev-fb-page-logo-white_2.png��5/       	 50        51   "   $

 Gray��63   $	 .50Q�52      $ 8Q�53      55      56      �� 57      $��1��6   
�������$��Center=�6?   .��6Z    Please Enter Your Product Key:6z   58      �� 59      -   5        $��
productKey��59   ?   $��1��6   $��20��6'   $��30q�63    �� 	MaxLength$ 16��6Q    �� TextAlignment$ Center=�6`   6y   � 5:      $��2��5;      $��10q�5<      $��Center=�5=       5>      �� 5?      $	��100 �6+   �� 5@      $	��100 �6+   .+�5D      �� -   5        $��btnLogin��5E      $��0��5D       �� 	IsDefault . $��Login��5G      $��15q�5I      	 5J        5K      $

 Gray��6/   $	 .50Q�5L      $ 8Q�5M      5O      �� 5P      -   5        $��	btnCancel��5Q      $��1��5P       �� IsCancel . $��Cancel��5S      $��15q�5U      	 5V        5W      $

 Gray��6/   $	 .50Q�5X      $ 8Q�5Y      5[      5\      5]      5_      5`      5a      