<script language="JavaScript">

function setVisibility(active, deactive) {
if(document.getElementById('active').value=='Add'){
document.getElementById('active').value = 'Add';
document.getElementById(active).style.display = 'inline';
document.getElementById(deactive).style.display = 'none';
}else{
document.getElementById('active').value = 'Add';
document.getElementById(active).style.display = 'none';
document.getElementById(deactive).style.display = 'inline';
}
}
</script>

