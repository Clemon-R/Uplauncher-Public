<?php
if (isset($_GET['file']))
{
	if (file_exists($_GET['file'])){
		echo md5_file($_GET['file']);
	}
}
?>