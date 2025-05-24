< Window x: Class = "CETWordMemory.MainWindow"
        xmlns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns: x = "http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns: d = "http://schemas.microsoft.com/expression/blend/2008"
        xmlns: mc = "http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns: local = "clr-namespace:CETWordMemory"
        mc: Ignorable = "d"
        Title = "四六级单词记忆助手" Height = "750" Width = "850"
        WindowStartupLocation = "CenterScreen"
        Background = "#EFEFEF" >

    < Window.Resources >
        < !--Basic Button Style -->
        <Style TargetType="Button" x:Key = "PrimaryButton" >
            < Setter Property = "Background" Value = "#4A6FA5" />
            < Setter Property = "Foreground" Value = "White" />
            < Setter Property = "FontSize" Value = "16" />
            < Setter Property = "Padding" Value = "15,10" />
            < Setter Property = "Margin" Value = "5" />
            < Setter Property = "BorderThickness" Value = "0" />
            < Setter Property = "Cursor" Value = "Hand" />
            < Setter Property = "Template" >
                < Setter.Value >
                    < ControlTemplate TargetType = "Button" >
                        < Border Background = "{TemplateBinding Background}" CornerRadius = "5" >
                            < ContentPresenter HorizontalAlignment = "Center" VerticalAlignment = "Center" />
                        </ Border >
                        < ControlTemplate.Triggers >
                            < Trigger Property = "IsMouseOver" Value = "True" >
                                < Setter Property = "Background" Value = "#3B5984" />
                            </ Trigger >
                            < Trigger Property = "IsEnabled" Value = "False" >
                                < Setter Property = "Background" Value = "#B0C4DE" />
                                < Setter Property = "Foreground" Value = "#777777" />
                            </ Trigger >
                        </ ControlTemplate.Triggers >
                    </ ControlTemplate >
                </ Setter.Value >
            </ Setter >
        </ Style >

        < Style TargetType = "Button" x: Key = "SecondaryButton" BasedOn = "{StaticResource PrimaryButton}" >
            < Setter Property = "Background" Value = "#E67E22" />
            < Setter Property = "Template" >
                < Setter.Value >
                    < ControlTemplate TargetType = "Button" >
                        < Border Background = "{TemplateBinding Background}" CornerRadius = "5" >
                            < ContentPresenter HorizontalAlignment = "Center" VerticalAlignment = "Center" />
                        </ Border >
                        < ControlTemplate.Triggers >
                            < Trigger Property = "IsMouseOver" Value = "True" >
                                < Setter Property = "Background" Value = "#D35400" />
                            </ Trigger >
                            < Trigger Property = "IsEnabled" Value = "False" >
                                < Setter Property = "Background" Value = "#FAD7A0" />
                                < Setter Property = "Foreground" Value = "#777777" />
                            </ Trigger >
                        </ ControlTemplate.Triggers >
                    </ ControlTemplate >
                </ Setter.Value >
            </ Setter >
        </ Style >

        < Style TargetType = "TextBlock" x: Key = "SectionTitle" >
            < Setter Property = "FontSize" Value = "20" />
            < Setter Property = "FontWeight" Value = "SemiBold" />
            < Setter Property = "Margin" Value = "0,0,0,10" />
            < Setter Property = "Foreground" Value = "#333333" />
        </ Style >
    </ Window.Resources >

    < Grid >
        < Grid.RowDefinitions >
            < RowDefinition Height = "Auto" />
            < !--Title Bar-- >
            < RowDefinition Height = "*" />
            < !--Tab Control Area -->
            <RowDefinition Height="Auto"/>
            <!-- Status Bar -->
        </Grid.RowDefinitions>

        <!-- 顶部标题区域 -->
        <Border Grid.Row="0" Background="#4A6FA5" Height="60" CornerRadius="0,0,10,10" Margin="0,0,0,10">
            <TextBlock Text="四六级单词记忆助手" FontSize="24" FontWeight="Bold"
                       Foreground="White" HorizontalAlignment="Center" VerticalAlignment="Center"/>
        </Border>

        <!-- TabControl for different sections -->
        <TabControl x:Name = "mainTabControl" Grid.Row = "1" Margin = "10" Background = "Transparent" BorderThickness = "0"
                    SelectionChanged = "MainTabControl_SelectionChanged" >
            < !--学习模式 Tab-- >
            < TabItem Header = "学习模式" FontSize = "16" Padding = "10,5" >
                < Grid Background = "#F5F5F5" >
                    < Grid.RowDefinitions >
                        < RowDefinition Height = "Auto" />
                        < !--Word and Definition -->
                        <RowDefinition Height="*"/>
                        <!-- Image -->
                        <RowDefinition Height="Auto"/>
                        <!-- Buttons -->
                    </Grid.RowDefinitions>

                    <!-- 单词显示区域 -->
                    <Border Grid.Row="0" Margin="20,10,20,10" Background="White" CornerRadius="10"
                            BorderBrush="#CCCCCC" BorderThickness="1" Padding="15">
                        <ScrollViewer VerticalScrollBarVisibility="Auto">
                            <StackPanel Orientation="Vertical">
                                <TextBlock x:Name = "txtWord" Text = "Word" FontSize = "36" FontWeight = "Bold"
                                           HorizontalAlignment = "Center" Foreground = "#333333" />
                                < TextBlock x: Name = "txtPhonetic" Text = "[Phonetic]" FontSize = "18"
                                           HorizontalAlignment = "Center" Foreground = "#666666" Margin = "0,5,0,10" />

                                < !--中文词义解释-- >
                                < TextBlock x: Name = "txtChineseDefinition" Text = "释义将在此显示..."
                                           FontSize = "18" TextWrapping = "Wrap" Margin = "0,10,0,10"
                                           Foreground = "#333333" FontWeight = "SemiBold" />

                                < !--例句-- >
                                < TextBlock x: Name = "txtExample" Text = "例句将在此显示..."
                                           FontSize = "14" TextWrapping = "Wrap" Margin = "0,10,0,0"
                                           Foreground = "#555555" />
                            </ StackPanel >
                        </ ScrollViewer >
                    </ Border >

                    < !--图片显示区域-- >
                    < Border Grid.Row = "1" Margin = "20,0,20,10" Background = "White" CornerRadius = "10"
                            BorderBrush = "#CCCCCC" BorderThickness = "1" MinHeight = "150" >
                        < Grid >
                            < Image x: Name = "imgWord" Stretch = "Uniform" Margin = "10"
                                   Source = "https://picsum.photos/600/150" />
                            < TextBlock x: Name = "txtImageLoading" Text = "图片加载中..." FontSize = "16"
                                       HorizontalAlignment = "Center" VerticalAlignment = "Center"
                                       Visibility = "Collapsed" />
                        </ Grid >
                    </ Border >

                    < !--操作按钮区域-- >
                    < StackPanel Grid.Row = "2" Orientation = "Horizontal" HorizontalAlignment = "Center" Margin = "0,10,0,10" >
                        < Button x: Name = "btnPrevious" Content = "上一个" Style = "{StaticResource PrimaryButton}"
                                Click = "BtnPrevious_Click" />
                        < Button x: Name = "btnNext" Content = "下一个" Style = "{StaticResource PrimaryButton}"
                                Click = "BtnNext_Click" />
                        < Button x: Name = "btnNewWord" Content = "新单词" Style = "{StaticResource SecondaryButton}"
                                Click = "BtnNewWord_Click" />
                        < Button x: Name = "btnMarkKnown" Content = "我会了" Style = "{StaticResource PrimaryButton}" Margin = "20,5,5,5"
                                Click = "BtnMarkKnown_Click" ToolTip = "将当前单词标记为已掌握/未掌握" />
                        < Button x: Name = "btnAddToMistakes" Content = "加入错题本" Style = "{StaticResource SecondaryButton}"
                                Click = "BtnAddToMistakes_Click" ToolTip = "手动将当前单词加入错题本" />
                    </ StackPanel >
                </ Grid >
            </ TabItem >

            < !--测试模式 Tab-- >
            < TabItem Header = "测试模式" FontSize = "16" Padding = "10,5" >
                < Grid Background = "#F5F5F5" Margin = "20" >
                    < Grid.RowDefinitions >
                        < RowDefinition Height = "Auto" />
                        < !--Question Type-- >
                        < RowDefinition Height = "Auto" />
                        < !--Question-- >
                        < RowDefinition Height = "Auto" />
                        < !--Answer Input-- >
                        < RowDefinition Height = "Auto" />
                        < !--Feedback-- >
                        < RowDefinition Height = "*" />
                        < !--Spacer-- >
                        < RowDefinition Height = "Auto" />
                        < !--Controls-- >
                        < RowDefinition Height = "Auto" />
                        < !--Score-- >
                    </ Grid.RowDefinitions >

                    < TextBlock x: Name = "txtTestQuestionType" Grid.Row = "0" Text = "测试类型: 看中文释义写单词" FontSize = "16" Foreground = "#555" Margin = "0,0,0,10" HorizontalAlignment = "Center" Style = "{StaticResource SectionTitle}" />

                    < Border Grid.Row = "1" Margin = "0,0,0,15" Background = "White" CornerRadius = "8" Padding = "20" BorderBrush = "#CCCCCC" BorderThickness = "1" MinHeight = "80" >
                        < ScrollViewer VerticalScrollBarVisibility = "Auto" >
                            < TextBlock x: Name = "txtTestQuestion" Text = "[中文释义将在此显示]" FontSize = "22" FontWeight = "Bold"
                                   TextWrapping = "Wrap" HorizontalAlignment = "Center" VerticalAlignment = "Center" Foreground = "#333333" />
                        </ ScrollViewer >
                    </ Border >

                    < TextBox x: Name = "txtTestAnswer" Grid.Row = "2" FontSize = "18" Margin = "0,0,0,15" Padding = "10"
                             BorderBrush = "#CCCCCC" BorderThickness = "1" MinHeight = "60" TextWrapping = "Wrap" VerticalScrollBarVisibility = "Auto"
                             KeyDown = "TxtTestAnswer_KeyDown" VerticalContentAlignment = "Center" />

                    < TextBlock x: Name = "txtTestFeedback" Grid.Row = "3" Text = "[在此显示反馈信息]" FontSize = "16"
                               HorizontalAlignment = "Center" Margin = "0,0,0,15" FontWeight = "SemiBold" TextWrapping = "Wrap" />

                    < StackPanel Grid.Row = "5" Orientation = "Horizontal" HorizontalAlignment = "Center" >
                        < Button x: Name = "btnTestSubmit" Content = "提交答案" Style = "{StaticResource PrimaryButton}"
                                Click = "BtnTestSubmit_Click" />
                        < Button x: Name = "btnTestShowAnswer" Content = "显示答案" Style = "{StaticResource SecondaryButton}"
                                Click = "BtnTestShowAnswer_Click" />
                        < Button x: Name = "btnTestNextQuestion" Content = "下一题" Style = "{StaticResource PrimaryButton}"
                                Click = "BtnTestNextQuestion_Click" IsEnabled = "False" />
                    </ StackPanel >

                    < TextBlock x: Name = "txtTestScore" Grid.Row = "6" Text = "得分: 0/0 (正确率: 0%)" FontSize = "14"
                               HorizontalAlignment = "Right" Margin = "0,15,0,0" Foreground = "#333" />
                </ Grid >
            </ TabItem >


            < !--错题本 Tab-- >
            < TabItem Header = "错题本" FontSize = "16" Padding = "10,5" >
                < Grid Background = "#F5F5F5" Margin = "20" >
                    < Grid.RowDefinitions >
                        < RowDefinition Height = "Auto" />
                        < !--Title & Controls-- >
                        < RowDefinition Height = "*" />
                        < !--List of Wrong Words -->
                    </Grid.RowDefinitions>

                    <StackPanel Grid.Row="0" Orientation="Horizontal" Margin="0,0,0,10">
                        <TextBlock Text="我的错题集" Style="{StaticResource SectionTitle}" VerticalAlignment="Center"/>
                        <Button x:Name = "btnReviewWrongWord" Content = "复习选中" Style = "{StaticResource PrimaryButton}"
                                Margin = "20,0,5,0" Click = "BtnReviewWrongWord_Click" IsEnabled = "False" ToolTip = "在学习模式中复习选中的单词" />
                        < Button x: Name = "btnRemoveFromWrongWords" Content = "移除选中" Style = "{StaticResource SecondaryButton}"
                                Click = "BtnRemoveFromWrongWords_Click" IsEnabled = "False" ToolTip = "从错题本中移除选中的单词" />
                        < Button x: Name = "btnClearWrongWords" Content = "清空错题本" Style = "{StaticResource SecondaryButton}" Background = "DarkRed"
                                Click = "BtnClearWrongWords_Click" Margin = "20,0,0,0" ToolTip = "清空所有错题记录" />
                    </ StackPanel >

                    < ListView x: Name = "lvWrongWords" Grid.Row = "1" BorderBrush = "#CCCCCC" BorderThickness = "1"
                              SelectionChanged = "LvWrongWords_SelectionChanged" AlternationCount = "2" >
                        < ListView.ItemContainerStyle >
                            < Style TargetType = "ListViewItem" >
                                < Style.Triggers >
                                    < Trigger Property = "ItemsControl.AlternationIndex" Value = "1" >
                                        < Setter Property = "Background" Value = "#F0F0F0" />
                                    </ Trigger >
                                </ Style.Triggers >
                            </ Style >
                        </ ListView.ItemContainerStyle >
                        < ListView.View >
                            < GridView >
                                < GridViewColumn Header = "单词" Width = "180" DisplayMemberBinding = "{Binding WordText}" />
                                < GridViewColumn Header = "音标" Width = "180" DisplayMemberBinding = "{Binding Phonetic}" />
                                < GridViewColumn Header = "中文释义" Width = "250" DisplayMemberBinding = "{Binding ChineseDefinition}" />
                                < GridViewColumn Header = "答错次数" Width = "100" DisplayMemberBinding = "{Binding TimesIncorrect}" />
                            </ GridView >
                        </ ListView.View >
                    </ ListView >
                </ Grid >
            </ TabItem >
        </ TabControl >

        < !--状态栏-- >
        < StatusBar Grid.Row = "2" Height = "30" VerticalAlignment = "Bottom" Background = "#DDDDDD" >
            < StatusBarItem >
                < TextBlock x: Name = "txtStatus" Text = "准备就绪" Foreground = "#333333" />
            </ StatusBarItem >
            < Separator />
            < StatusBarItem DockPanel.Dock = "Right" >
                < TextBlock x: Name = "txtWordCount" Text = "词库: 0 / 总计: 0 | 错题: 0" Foreground = "#333333" Margin = "0,0,10,0" />
            </ StatusBarItem >
        </ StatusBar >

        < !--错误提示对话框-- >
        < Popup x: Name = "errorPopup" Placement = "Center" AllowsTransparency = "True"
               IsOpen = "False" StaysOpen = "False" >
            < Border Background = "White" BorderBrush = "Red" BorderThickness = "2"
                    CornerRadius = "5" Padding = "20" Width = "400" MaxHeight = "300" >
                < Border.Effect >
                    < DropShadowEffect ShadowDepth = "4" Direction = "320" Color = "Black" Opacity = "0.5" BlurRadius = "5" />
                </ Border.Effect >
                < Grid >
                    < Grid.RowDefinitions >
                        < RowDefinition Height = "Auto" />
                        < RowDefinition Height = "*" />
                        < RowDefinition Height = "Auto" />
                    </ Grid.RowDefinitions >
                    < TextBlock Grid.Row = "0" Text = "错误提示" FontSize = "18" FontWeight = "Bold" Foreground = "Red" Margin = "0,0,0,10" />
                    < ScrollViewer Grid.Row = "1" VerticalScrollBarVisibility = "Auto" Margin = "0,0,0,15" >
                        < TextBlock x: Name = "txtError" Text = "Error message will appear here." TextWrapping = "Wrap" />
                    </ ScrollViewer >
                    < Button Grid.Row = "2" Content = "确定" Width = "80" HorizontalAlignment = "Right" Click = "BtnCloseError_Click" Style = "{StaticResource PrimaryButton}" />
                </ Grid >
            </ Border >
        </ Popup >

        < !--Loading Indicator Popup -->
        <Popup x:Name = "loadingIndicatorPopup" Placement = "Center" AllowsTransparency = "True" IsOpen = "False" StaysOpen = "True" >
            < Border Background = "#D2000000" CornerRadius = "8" Padding = "25" MinWidth = "220" MaxWidth = "400"
                    BorderBrush = "#555555" BorderThickness = "1" >
                < Border.Effect >
                    < DropShadowEffect ShadowDepth = "2" BlurRadius = "10" Opacity = "0.3" />
                </ Border.Effect >
                < StackPanel Orientation = "Vertical" >
                    < TextBlock x: Name = "loadingIndicatorPopupText" Text = "加载中，请稍候..." FontSize = "16"
                               Foreground = "White" HorizontalAlignment = "Center" VerticalAlignment = "Center"
                               TextWrapping = "Wrap" Margin = "0,0,0,15" />
                    < ProgressBar x: Name = "loadingIndicatorProgressBar" IsIndeterminate = "True" Height = "10" Width = "Auto" MinWidth = "150"
                                 Style = "{StaticResource {x:Type ProgressBar}}" />
                </ StackPanel >
            </ Border >
        </ Popup >

    </ Grid >
</ Window >