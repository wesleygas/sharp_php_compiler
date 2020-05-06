<?php
{
    $x = 1;
    $y = $x or True;
    $z = "x: ";
    echo $x + $y;
    echo $z . $x;
    echo $x * $z; /* ERROR */
}
?>